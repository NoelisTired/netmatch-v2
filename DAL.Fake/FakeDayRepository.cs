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
        // Offerte 1: Rondreis Toscane (4 dagen)
        new DayDto
        {
            Id = 1, QuoteId = 1, DayNumber = 1, Date = DateTime.Today.AddDays(14),
            Title = "Aankomst Florence",
            Description = "Ochtendvlucht vanuit Amsterdam Schiphol naar Florence (Peretola). " +
                          "Privétransfer naar het hotel in het centrum. Na het inchecken vrije " +
                          "middag om de stad te verkennen: de Ponte Vecchio, Piazza della Signoria " +
                          "en een eerste Italiaans aperitivo.",
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },
        new DayDto
        {
            Id = 2, QuoteId = 1, DayNumber = 2, Date = DateTime.Today.AddDays(15),
            Title = "Florence: kunst en cultuur",
            Description = "Ochtend: skip-the-line toegang tot de Galleria degli Uffizi met " +
                          "Engelstalige gids (ca. 2,5 uur). Lunch in de wijk San Lorenzo. " +
                          "Middag: beklimming van de Duomo-koepel van Brunelleschi (463 treden) " +
                          "voor een 360°-panorama over de stad. Avond vrij.",
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-3)
        },
        new DayDto
        {
            Id = 3, QuoteId = 1, DayNumber = 3, Date = DateTime.Today.AddDays(16),
            Title = "Florence → Siena",
            Description = "Check-out in Florence. Treinreis naar Siena (ca. 1u30). " +
                          "Inchecken bij Palazzo Ravizza. Begeleide wandeling door de " +
                          "middeleeuwse binnenstad: Piazza del Campo, de Duomo en het " +
                          "Baptisterium. Avondeten in een traditionele trattoria.",
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },
        new DayDto
        {
            Id = 4, QuoteId = 1, DayNumber = 4, Date = DateTime.Today.AddDays(17),
            Title = "Siena & terugreis",
            Description = "Vrije ochtend voor winkelen of een bezoek aan het Museo Civico. " +
                          "Middagtransfer naar Florence (FLR) voor de terugvlucht naar Amsterdam. " +
                          "Verwachte aankomst Schiphol in de avond.",
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },

        // Offerte 2: City trip Barcelona (2 dagen)
        new DayDto
        {
            Id = 5, QuoteId = 2, DayNumber = 1, Date = DateTime.Today.AddDays(30),
            Title = "Aankomst Barcelona",
            Description = "Middagvlucht naar Barcelona El Prat. Privétransfer naar het hotel. " +
                          "Avondwandeling over La Rambla richting de haven, met tapas in " +
                          "de wijk El Born.",
            CreatedAt = DateTime.Now.AddDays(-5), UpdatedAt = DateTime.Now.AddDays(-5)
        },
        new DayDto
        {
            Id = 6, QuoteId = 2, DayNumber = 2, Date = DateTime.Today.AddDays(31),
            Title = "Gaudí & Gotische wijk",
            Description = "Ochtend: rondleiding Sagrada Família met skip-the-line tickets " +
                          "(incl. torenbeklimming). Lunch in Eixample. Middag: Park Güell " +
                          "en de Gotische wijk. Terugvlucht in de avond.",
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
