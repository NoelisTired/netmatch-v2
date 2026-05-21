using Interface.DataInterfaces;
using Interface.DTO;
using Logic.Enums;
using Logic.Models;

namespace Logic;

/// <summary>
/// Bedrijfslogica rond offertes (FR-01). Vertaalt tussen DTO's (datalaag) en
/// het <see cref="Quote"/> domeinmodel en bewaakt de validatieregels. De
/// presentatielaag krijgt nette foutmeldingen terug i.p.v. excepties.
/// </summary>
public class QuoteService
{
    private readonly IQuoteRepository _quoteRepository;

    public QuoteService(IQuoteRepository quoteRepository)
    {
        _quoteRepository = quoteRepository;
    }

    public IEnumerable<Quote> GetAllQuotes()
    {
        return _quoteRepository.GetAll().Select(MapToModel);
    }

    /// <summary>FR-09: alleen offertes van het opgegeven reisbureau.</summary>
    public IEnumerable<Quote> GetQuotesForTravelAgent(int travelAgentId)
    {
        return _quoteRepository.GetAllByTravelAgent(travelAgentId).Select(MapToModel);
    }

    public Quote? GetQuoteById(int id)
    {
        var dto = _quoteRepository.GetById(id);
        return dto is null ? null : MapToModel(dto);
    }

    /// <summary>
    /// Maakt een nieuwe (concept-)offerte aan. FR-01: titel en taal zijn
    /// verplicht en de taal moet ondersteund zijn (TC-01, TC-02).
    /// </summary>
    public (string? Error, int Id) CreateQuote(int travelAgentId, string? title, string? language)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(language))
        {
            return ("Titel en taal zijn verplicht.", 0);
        }

        if (!QuoteEnumMapping.TryParseLanguage(language, out var parsedLanguage))
        {
            return ("Kies een ondersteunde taal: Nederlands, Engels, Duits of Frans.", 0);
        }

        var quote = new Quote(travelAgentId, title, parsedLanguage);

        var id = _quoteRepository.Add(new CreateQuoteDto
        {
            TravelAgentId = quote.TravelAgentId,
            Title = quote.Title,
            Language = quote.Language.ToCode(),
            Status = quote.Status.ToCode()
        });

        return (null, id);
    }

    /// <summary>Werkt een bestaande offerte bij met dezelfde validatie als bij aanmaken.</summary>
    public string? UpdateQuote(int id, string? title, string? language, string? status)
    {
        var existing = _quoteRepository.GetById(id);
        if (existing is null)
        {
            return "Offerte niet gevonden.";
        }

        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(language))
        {
            return "Titel en taal zijn verplicht.";
        }

        if (!QuoteEnumMapping.TryParseLanguage(language, out var parsedLanguage))
        {
            return "Kies een ondersteunde taal: Nederlands, Engels, Duits of Frans.";
        }

        var parsedStatus = QuoteEnumMapping.ParseStatus(status);

        _quoteRepository.Update(id, new CreateQuoteDto
        {
            TravelAgentId = existing.TravelAgentId,
            Title = title.Trim(),
            Language = parsedLanguage.ToCode(),
            Status = parsedStatus.ToCode()
        });

        return null;
    }

    public void DeleteQuote(int id)
    {
        _quoteRepository.SoftDelete(id);
    }

    private static Quote MapToModel(QuoteDto dto)
    {
        QuoteEnumMapping.TryParseLanguage(dto.Language, out var language);
        var status = QuoteEnumMapping.ParseStatus(dto.Status);
        return new Quote(dto.Id, dto.TravelAgentId, dto.Title, language, status,
            dto.CreatedAt, dto.UpdatedAt);
    }
}
