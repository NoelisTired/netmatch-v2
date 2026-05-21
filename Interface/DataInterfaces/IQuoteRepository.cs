using Interface.DTO;

namespace Interface.DataInterfaces;

/// <summary>
/// Datatoegang voor offertes (tabel <c>offerte</c>). Implementaties gebruiken
/// soft-delete: rijen met <c>deleted_at</c> gevuld worden nooit teruggegeven.
/// </summary>
public interface IQuoteRepository
{
    IEnumerable<QuoteDto> GetAll();

    /// <summary>Offertes van één reisbureau (FR-09: data-isolatie per TravelAgent).</summary>
    IEnumerable<QuoteDto> GetAllByTravelAgent(int travelAgentId);

    QuoteDto? GetById(int id);

    /// <summary>Voegt een offerte toe en geeft het gegenereerde id terug.</summary>
    int Add(CreateQuoteDto dto);

    void Update(int id, CreateQuoteDto dto);

    /// <summary>Soft-delete: zet <c>deleted_at</c> in plaats van fysiek verwijderen.</summary>
    void SoftDelete(int id);
}
