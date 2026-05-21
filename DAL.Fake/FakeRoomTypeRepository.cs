using Interface.DataInterfaces;
using Interface.DTO;

namespace DAL.Memory;

/// <summary>
/// In-memory implementatie van <see cref="IRoomTypeRepository"/>. Geseed met
/// kamertypes voor de demo-accommodatie (AccommodationId 1).
/// </summary>
public class FakeRoomTypeRepository : IRoomTypeRepository
{
    private readonly List<RoomTypeDto> _items = new()
    {
        // Hotel Brunelleschi (Florence)
        new RoomTypeDto
        {
            Id = 1, AccommodationId = 1, Name = "Tweepersoonskamer",
            PricePerNight = 145m, Capacity = 2,
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },
        new RoomTypeDto
        {
            Id = 2, AccommodationId = 1, Name = "Junior Suite",
            PricePerNight = 260m, Capacity = 3,
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },

        // Palazzo Ravizza (Siena)
        new RoomTypeDto
        {
            Id = 3, AccommodationId = 2, Name = "Classic",
            PricePerNight = 120m, Capacity = 2,
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },
        new RoomTypeDto
        {
            Id = 4, AccommodationId = 2, Name = "Superior",
            PricePerNight = 175m, Capacity = 2,
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },

        // Hotel Catalonia (Barcelona)
        new RoomTypeDto
        {
            Id = 5, AccommodationId = 3, Name = "Double Room",
            PricePerNight = 130m, Capacity = 2,
            CreatedAt = DateTime.Now.AddDays(-5), UpdatedAt = DateTime.Now.AddDays(-5)
        }
    };
    private int _nextId = 6;

    public IEnumerable<RoomTypeDto> GetByAccommodation(int accommodationId) =>
        _items.Where(r => r.AccommodationId == accommodationId).OrderBy(r => r.Id).ToList();

    public RoomTypeDto? GetById(int id) => _items.FirstOrDefault(r => r.Id == id);

    public int Add(CreateRoomTypeDto dto)
    {
        var id = _nextId++;
        _items.Add(new RoomTypeDto
        {
            Id = id,
            AccommodationId = dto.AccommodationId,
            Name = dto.Name,
            PricePerNight = dto.PricePerNight,
            Capacity = dto.Capacity,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });
        return id;
    }

    public void Update(int id, CreateRoomTypeDto dto)
    {
        var index = _items.FindIndex(r => r.Id == id);
        if (index < 0)
        {
            return;
        }

        _items[index] = _items[index] with
        {
            Name = dto.Name,
            PricePerNight = dto.PricePerNight,
            Capacity = dto.Capacity,
            UpdatedAt = DateTime.Now
        };
    }

    public void SoftDelete(int id) => _items.RemoveAll(r => r.Id == id);
}
