using Interface.DataInterfaces;
using Interface.DTO;

namespace DAL.Memory;

/// <summary>
/// In-memory implementatie van <see cref="ITransportRepository"/>. Geseed met
/// transport voor dag 1 van de demo-offerte.
/// </summary>
public class FakeTransportRepository : ITransportRepository
{
    private readonly List<TransportDto> _transports = new()
    {
        new TransportDto
        {
            Id = 1, DayId = 1, Type = "Vlucht",
            DepartureLocation = "Amsterdam (AMS)", ArrivalLocation = "Florence (FLR)",
            FlightNumber = "KL1653", Airline = "KLM",
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },
        new TransportDto
        {
            Id = 2, DayId = 3, Type = "Trein",
            DepartureLocation = "Florence S.M.N.", ArrivalLocation = "Siena",
            FlightNumber = null, Airline = null,
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },
        new TransportDto
        {
            Id = 3, DayId = 4, Type = "Vlucht",
            DepartureLocation = "Florence (FLR)", ArrivalLocation = "Amsterdam (AMS)",
            FlightNumber = "KL1654", Airline = "KLM",
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },
        new TransportDto
        {
            Id = 4, DayId = 5, Type = "Vlucht",
            DepartureLocation = "Amsterdam (AMS)", ArrivalLocation = "Barcelona (BCN)",
            FlightNumber = "VY8301", Airline = "Vueling",
            CreatedAt = DateTime.Now.AddDays(-5), UpdatedAt = DateTime.Now.AddDays(-5)
        }
    };
    private int _nextId = 5;

    public IEnumerable<TransportDto> GetByDay(int dayId) =>
        _transports.Where(t => t.DayId == dayId).OrderBy(t => t.Id).ToList();

    public TransportDto? GetById(int id) => _transports.FirstOrDefault(t => t.Id == id);

    public int Add(CreateTransportDto dto)
    {
        var id = _nextId++;
        _transports.Add(new TransportDto
        {
            Id = id,
            DayId = dto.DayId,
            Type = dto.Type,
            DepartureLocation = dto.DepartureLocation,
            ArrivalLocation = dto.ArrivalLocation,
            FlightNumber = dto.FlightNumber,
            Airline = dto.Airline,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        });
        return id;
    }

    public void Update(int id, CreateTransportDto dto)
    {
        var index = _transports.FindIndex(t => t.Id == id);
        if (index < 0)
        {
            return;
        }

        _transports[index] = _transports[index] with
        {
            Type = dto.Type,
            DepartureLocation = dto.DepartureLocation,
            ArrivalLocation = dto.ArrivalLocation,
            FlightNumber = dto.FlightNumber,
            Airline = dto.Airline,
            UpdatedAt = DateTime.Now
        };
    }

    public void SoftDelete(int id) => _transports.RemoveAll(t => t.Id == id);
}
