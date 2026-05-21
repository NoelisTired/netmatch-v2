using Interface.DataInterfaces;
using Interface.DTO;

namespace DAL.Memory;

/// <summary>
/// In-memory implementatie van <see cref="IDayRepository"/>. Geseed met
/// dagen voor de demo-offerte (QuoteId 1).
/// </summary>
public class FakeDayRepository : IDayRepository
{
    private readonly List<DayDto> _days = new()
    {
        // Offerte 1 — Rondreis Toscane (4 dagen)
        new DayDto
        {
            Id = 1, QuoteId = 1, DayNumber = 1, Date = DateTime.Today.AddDays(14),
            Title = "Aankomst Florence", Description = "Vlucht naar Florence, transfer en vrije middag.",
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },
        new DayDto
        {
            Id = 2, QuoteId = 1, DayNumber = 2, Date = DateTime.Today.AddDays(15),
            Title = "Florence – musea", Description = "Gegidste tour langs de Duomo en de Uffizi.",
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-3)
        },
        new DayDto
        {
            Id = 3, QuoteId = 1, DayNumber = 3, Date = DateTime.Today.AddDays(16),
            Title = "Florence → Siena", Description = "Treinreis naar Siena, incheck en stadswandeling.",
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },
        new DayDto
        {
            Id = 4, QuoteId = 1, DayNumber = 4, Date = DateTime.Today.AddDays(17),
            Title = "Siena & terugreis", Description = "Ochtend vrij, daarna transfer en vlucht naar huis.",
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },

        // Offerte 2 — City trip Barcelona (2 dagen)
        new DayDto
        {
            Id = 5, QuoteId = 2, DayNumber = 1, Date = DateTime.Today.AddDays(30),
            Title = "Aankomst Barcelona", Description = "Vlucht naar Barcelona en avond op de Ramblas.",
            CreatedAt = DateTime.Now.AddDays(-5), UpdatedAt = DateTime.Now.AddDays(-5)
        },
        new DayDto
        {
            Id = 6, QuoteId = 2, DayNumber = 2, Date = DateTime.Today.AddDays(31),
            Title = "Gaudí-dag", Description = "Sagrada Família en Park Güell met gids.",
            CreatedAt = DateTime.Now.AddDays(-5), UpdatedAt = DateTime.Now.AddDays(-2)
        }
    };
    private int _nextId = 7;

    public IEnumerable<DayDto> GetByQuote(int quoteId) =>
        _days.Where(d => d.QuoteId == quoteId).OrderBy(d => d.DayNumber).ToList();

    public DayDto? GetById(int id) => _days.FirstOrDefault(d => d.Id == id);

    public int Add(CreateDayDto dto)
    {
        var id = _nextId++;
        _days.Add(new DayDto
        {
            Id = id,
            QuoteId = dto.QuoteId,
            DayNumber = dto.DayNumber,
            Date = dto.Date,
            Title = dto.Title,
            Description = dto.Description,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });
        return id;
    }

    public void Update(int id, CreateDayDto dto)
    {
        var index = _days.FindIndex(d => d.Id == id);
        if (index < 0)
        {
            return;
        }

        _days[index] = _days[index] with
        {
            DayNumber = dto.DayNumber,
            Date = dto.Date,
            Title = dto.Title,
            Description = dto.Description,
            UpdatedAt = DateTime.Now
        };
    }

    public void SoftDelete(int id) => _days.RemoveAll(d => d.Id == id);
}
