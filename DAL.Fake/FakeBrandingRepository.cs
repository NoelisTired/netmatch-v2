using Interface.DataInterfaces;
using Interface.DTO;

namespace DAL.Memory;

/// <summary>
/// In-memory implementatie van <see cref="IBrandingRepository"/>. Geseed met
/// een standaard-huisstijl voor het demo-reisbureau (TravelAgentId 1).
/// </summary>
public class FakeBrandingRepository : IBrandingRepository
{
    private readonly List<BrandingDto> _items = new()
    {
        new BrandingDto
        {
            Id = 1, TravelAgentId = 1, LogoPath = null,
            PrimaryColor = "#1d4ed8", AccentColor = "#f59e0b",
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        }
    };
    private int _nextId = 2;

    public BrandingDto? GetByTravelAgent(int travelAgentId) =>
        _items.FirstOrDefault(b => b.TravelAgentId == travelAgentId);

    public void Save(CreateBrandingDto dto)
    {
        var index = _items.FindIndex(b => b.TravelAgentId == dto.TravelAgentId);
        if (index >= 0)
        {
            _items[index] = _items[index] with
            {
                LogoPath = dto.LogoPath,
                PrimaryColor = dto.PrimaryColor,
                AccentColor = dto.AccentColor,
                UpdatedAt = DateTime.Now
            };
            return;
        }

        _items.Add(new BrandingDto
        {
            Id = _nextId++,
            TravelAgentId = dto.TravelAgentId,
            LogoPath = dto.LogoPath,
            PrimaryColor = dto.PrimaryColor,
            AccentColor = dto.AccentColor,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });
    }
}
