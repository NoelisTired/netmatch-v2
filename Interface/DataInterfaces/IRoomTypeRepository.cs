using Interface.DTO;

namespace Interface.DataInterfaces;

/// <summary>
/// Datatoegang voor kamertypes (tabel <c>kamertype</c>). Soft-delete:
/// rijen met <c>deleted_at</c> gevuld komen nooit terug.
/// </summary>
public interface IRoomTypeRepository
{
    /// <summary>Alle kamertypes van één accommodatie.</summary>
    IEnumerable<RoomTypeDto> GetByAccommodation(int accommodationId);

    RoomTypeDto? GetById(int id);

    /// <summary>Voegt een kamertype toe en geeft het gegenereerde id terug.</summary>
    int Add(CreateRoomTypeDto dto);

    void Update(int id, CreateRoomTypeDto dto);

    void SoftDelete(int id);
}
