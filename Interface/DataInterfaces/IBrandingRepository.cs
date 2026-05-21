using Interface.DTO;

namespace Interface.DataInterfaces;

/// <summary>
/// Datatoegang voor de huisstijl per reisbureau (tabel <c>branding</c>,
/// 1:1 met travelagent).
/// </summary>
public interface IBrandingRepository
{
    /// <summary>Huisstijl van één reisbureau, of null als die nog niet is ingesteld.</summary>
    BrandingDto? GetByTravelAgent(int travelAgentId);

    /// <summary>INSERT bij eerste keer, anders UPDATE (1:1-relatie).</summary>
    void Save(CreateBrandingDto dto);
}
