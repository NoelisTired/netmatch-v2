using Interface.DataInterfaces;
using Interface.DTO;

namespace DAL.Memory;

/// <summary>
/// In-memory implementatie van <see cref="IQuoteRepository"/> zodat de app
/// lokaal draait zonder database (DataProvider=Fake). Geseed met demodata.
/// Soft-delete = uit de lijst halen (gedrag identiek aan de SQL-variant).
/// </summary>
public class FakeQuoteRepository : IQuoteRepository
{
    private readonly List<QuoteDto> _quotes = new()
    {
        new QuoteDto
        {
            Id = 1, TravelAgentId = 1, Title = "Rondreis Toscane", Language = "NL",
            Status = "concept", CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-2)
        },
        new QuoteDto
        {
            Id = 2, TravelAgentId = 1, Title = "City trip Barcelona", Language = "EN",
            Status = "definitief", CreatedAt = DateTime.Now.AddDays(-5), UpdatedAt = DateTime.Now.AddDays(-1)
        }
    };
    private int _nextId = 3;

    public IEnumerable<QuoteDto> GetAll() => _quotes.OrderByDescending(q => q.Id).ToList();

    public IEnumerable<QuoteDto> GetAllByTravelAgent(int travelAgentId) =>
        _quotes.Where(q => q.TravelAgentId == travelAgentId)
            .OrderByDescending(q => q.Id).ToList();

    public QuoteDto? GetById(int id) => _quotes.FirstOrDefault(q => q.Id == id);

    public int Add(CreateQuoteDto dto)
    {
        var id = _nextId++;
        _quotes.Add(new QuoteDto
        {
            Id = id,
            TravelAgentId = dto.TravelAgentId,
            Title = dto.Title,
            Language = dto.Language,
            Status = dto.Status,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });
        return id;
    }

    public void Update(int id, CreateQuoteDto dto)
    {
        var index = _quotes.FindIndex(q => q.Id == id);
        if (index < 0)
        {
            return;
        }

        _quotes[index] = _quotes[index] with
        {
            Title = dto.Title,
            Language = dto.Language,
            Status = dto.Status,
            UpdatedAt = DateTime.Now
        };
    }

    public void SoftDelete(int id) => _quotes.RemoveAll(q => q.Id == id);
}
