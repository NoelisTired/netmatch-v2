using Interface.DataInterfaces;
using Interface.DTO;

namespace DAL.Memory;

/// <summary>
/// In-memory implementatie van <see cref="IAccommodationRepository"/>. Geseed
/// met een accommodatie voor dag 1 van de demo-offerte.
/// </summary>
public class FakeAccommodationRepository : IAccommodationRepository
{
    private readonly List<AccommodationDto> _items = new()
    {
        new AccommodationDto
        {
            Id = 1, DayId = 1, Name = "Hotel Brunelleschi",
            Address = "Piazza Santa Elisabetta 3, Florence",
            Description = "4-sterrenhotel in het historische centrum.",
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },
        new AccommodationDto
        {
            Id = 2, DayId = 3, Name = "Palazzo Ravizza",
            Address = "Pian dei Mantellini 34, Siena",
            Description = "Historisch palazzo met tuin nabij het centrum.",
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },
        new AccommodationDto
        {
            Id = 3, DayId = 5, Name = "Hotel Catalonia Plaça Catalunya",
            Address = "Carrer de Bergara 11, Barcelona",
            Description = "Centraal hotel met dakterras en zwembad.",
            CreatedAt = DateTime.Now.AddDays(-5), UpdatedAt = DateTime.Now.AddDays(-5)
        }
    };
    private int _nextId = 4;

    public IEnumerable<AccommodationDto> GetByDay(int dayId) =>
        _items.Where(a => a.DayId == dayId).OrderBy(a => a.Id).ToList();

    public AccommodationDto? GetById(int id) => _items.FirstOrDefault(a => a.Id == id);

    public int Add(CreateAccommodationDto dto)
    {
        var id = _nextId++;
        _items.Add(new AccommodationDto
        {
            Id = id,
            DayId = dto.DayId,
            Name = dto.Name,
            Address = dto.Address,
            Description = dto.Description,
            ImagePath = dto.ImagePath,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });
        return id;
    }

    public void Update(int id, CreateAccommodationDto dto)
    {
        var index = _items.FindIndex(a => a.Id == id);
        if (index < 0)
        {
            return;
        }

        _items[index] = _items[index] with
        {
            Name = dto.Name,
            Address = dto.Address,
            Description = dto.Description,
            ImagePath = dto.ImagePath,
            UpdatedAt = DateTime.Now
        };
    }

    public void SoftDelete(int id) => _items.RemoveAll(a => a.Id == id);
}
