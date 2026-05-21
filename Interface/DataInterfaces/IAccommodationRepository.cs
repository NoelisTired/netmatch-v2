using Interface.DTO;

namespace Interface.DataInterfaces;

/// <summary>
/// Datatoegang voor accommodaties (tabel <c>accommodatie</c>). Soft-delete:
/// rijen met <c>deleted_at</c> gevuld komen nooit terug.
/// </summary>
public interface IAccommodationRepository
{
    /// <summary>Alle accommodaties van één reisdag.</summary>
    IEnumerable<AccommodationDto> GetByDay(int dayId);

    AccommodationDto? GetById(int id);

    /// <summary>Voegt een accommodatie toe en geeft het gegenereerde id terug.</summary>
    int Add(CreateAccommodationDto dto);

    void Update(int id, CreateAccommodationDto dto);

    void SoftDelete(int id);
}
