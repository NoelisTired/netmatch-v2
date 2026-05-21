using Interface.DataInterfaces;
using Interface.DTO;
using Logic.Models;

namespace Logic;

/// <summary>
/// Bedrijfslogica rond reisdagen (FR-02). Vertaalt tussen DTO's en het
/// <see cref="Day"/> domeinmodel en bewaakt de validatieregels. Geeft nette
/// foutmeldingen terug i.p.v. excepties.
/// </summary>
public class DayService
{
    private readonly IDayRepository _dayRepository;

    public DayService(IDayRepository dayRepository)
    {
        _dayRepository = dayRepository;
    }

    public IEnumerable<Day> GetDaysForQuote(int quoteId)
    {
        return _dayRepository.GetByQuote(quoteId).Select(MapToModel);
    }

    public Day? GetDayById(int id)
    {
        var dto = _dayRepository.GetById(id);
        return dto is null ? null : MapToModel(dto);
    }

    /// <summary>
    /// Voegt een dag toe aan een offerte. FR-02: dagnummer en datum zijn
    /// verplicht (TC-04).
    /// </summary>
    public (string? Error, int Id) AddDay(int quoteId, int? dayNumber, DateTime? date,
        string? title, string? description)
    {
        if (dayNumber is null or <= 0 || date is null)
        {
            return ("Dagnummer en datum zijn verplicht.", 0);
        }

        var day = new Day(quoteId, dayNumber.Value, date.Value, title, description);

        var id = _dayRepository.Add(new CreateDayDto
        {
            QuoteId = day.QuoteId,
            DayNumber = day.DayNumber,
            Date = day.Date,
            Title = day.Title,
            Description = day.Description
        });

        return (null, id);
    }

    public string? UpdateDay(int id, int? dayNumber, DateTime? date, string? title, string? description)
    {
        var existing = _dayRepository.GetById(id);
        if (existing is null)
        {
            return "Reisdag niet gevonden.";
        }

        if (dayNumber is null or <= 0 || date is null)
        {
            return "Dagnummer en datum zijn verplicht.";
        }

        _dayRepository.Update(id, new CreateDayDto
        {
            QuoteId = existing.QuoteId,
            DayNumber = dayNumber.Value,
            Date = date.Value,
            Title = string.IsNullOrWhiteSpace(title) ? null : title.Trim(),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim()
        });

        return null;
    }

    public void DeleteDay(int id)
    {
        _dayRepository.SoftDelete(id);
    }

    private static Day MapToModel(DayDto dto)
    {
        return new Day(dto.Id, dto.QuoteId, dto.DayNumber, dto.Date, dto.Title,
            dto.Description, dto.CreatedAt, dto.UpdatedAt);
    }
}
