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
            DepartureLocation = "Amsterdam Schiphol (AMS)", ArrivalLocation = "Florence Peretola (FLR)",
            FlightNumber = "KL1653", Airline = "KLM Royal Dutch Airlines", Price = 189.00m,
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },
        new TransportDto
        {
            Id = 2, DayId = 1, Type = "Privétransfer",
            DepartureLocation = "Florence Peretola (FLR)", ArrivalLocation = "Hotel Brunelleschi, centrum",
            FlightNumber = null, Airline = null, Price = 45.00m,
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },
        new TransportDto
        {
            Id = 3, DayId = 3, Type = "Trein",
            DepartureLocation = "Firenze Santa Maria Novella", ArrivalLocation = "Siena Piazza Gramsci",
            FlightNumber = null, Airline = null, Price = 12.50m,
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },
        new TransportDto
        {
            Id = 4, DayId = 4, Type = "Privétransfer",
            DepartureLocation = "Palazzo Ravizza, Siena", ArrivalLocation = "Florence Peretola (FLR)",
            FlightNumber = null, Airline = null, Price = 85.00m,
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },
        new TransportDto
        {
            Id = 5, DayId = 4, Type = "Vlucht",
            DepartureLocation = "Florence Peretola (FLR)", ArrivalLocation = "Amsterdam Schiphol (AMS)",
            FlightNumber = "KL1654", Airline = "KLM Royal Dutch Airlines", Price = 215.00m,
            CreatedAt = DateTime.Now.AddDays(-9), UpdatedAt = DateTime.Now.AddDays(-9)
        },
        new TransportDto
        {
            Id = 6, DayId = 5, Type = "Vlucht",
            DepartureLocation = "Amsterdam Schiphol (AMS)", ArrivalLocation = "Barcelona El Prat (BCN)",
            FlightNumber = "VY8301", Airline = "Vueling Airlines", Price = 79.00m,
            CreatedAt = DateTime.Now.AddDays(-5), UpdatedAt = DateTime.Now.AddDays(-5)
        },
        new TransportDto
        {
            Id = 7, DayId = 6, Type = "Vlucht",
            DepartureLocation = "Barcelona El Prat (BCN)", ArrivalLocation = "Amsterdam Schiphol (AMS)",
            FlightNumber = "VY8302", Airline = "Vueling Airlines", Price = 89.00m,
            CreatedAt = DateTime.Now.AddDays(-5), UpdatedAt = DateTime.Now.AddDays(-5)
        }
    };
    private int _nextId = 8;

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
            Price = dto.Price,
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
            Price = dto.Price,
            UpdatedAt = DateTime.Now
        };
    }

    public void SoftDelete(int id) => _transports.RemoveAll(t => t.Id == id);
}
