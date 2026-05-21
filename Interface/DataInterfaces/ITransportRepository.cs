using Interface.DTO;

namespace Interface.DataInterfaces;

/// <summary>
/// Datatoegang voor transportmiddelen (tabel <c>transport</c>). Soft-delete:
/// rijen met <c>deleted_at</c> gevuld komen nooit terug.
/// </summary>
public interface ITransportRepository
{
    /// <summary>Alle transportmiddelen van één reisdag.</summary>
    IEnumerable<TransportDto> GetByDay(int dayId);

    TransportDto? GetById(int id);

    /// <summary>Voegt transport toe en geeft het gegenereerde id terug.</summary>
    int Add(CreateTransportDto dto);

    void Update(int id, CreateTransportDto dto);

    void SoftDelete(int id);
}
