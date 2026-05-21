using Interface.DTO;

namespace Interface.DataInterfaces;

/// <summary>
/// Datatoegang voor reisdagen (tabel <c>dag</c>). Soft-delete: rijen met
/// <c>deleted_at</c> gevuld komen nooit terug.
/// </summary>
public interface IDayRepository
{
    /// <summary>Alle dagen van één offerte, op dagnummer gesorteerd.</summary>
    IEnumerable<DayDto> GetByQuote(int quoteId);

    DayDto? GetById(int id);

    /// <summary>Voegt een dag toe en geeft het gegenereerde id terug.</summary>
    int Add(CreateDayDto dto);

    void Update(int id, CreateDayDto dto);

    void SoftDelete(int id);
}
