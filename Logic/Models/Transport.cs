using Logic.Enums;

namespace Logic.Models;

/// <summary>
/// Domeinmodel van een transportmiddel binnen een reisdag (FR-03). Bewaakt
/// de invarianten: vertrek- en aankomstlocatie zijn altijd verplicht, en bij
/// een vlucht zijn vluchtnummer en luchtvaartmaatschappij verplicht.
/// </summary>
public class Transport
{
    public int Id { get; private set; }
    public int DayId { get; private set; }
    public TransportType Type { get; private set; }
    public string DepartureLocation { get; private set; }
    public string ArrivalLocation { get; private set; }
    public string? FlightNumber { get; private set; }
    public string? Airline { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    /// <summary>Nieuw transport (nog niet opgeslagen).</summary>
    public Transport(int dayId, TransportType type, string? departureLocation,
        string? arrivalLocation, string? flightNumber, string? airline)
    {
        if (string.IsNullOrWhiteSpace(departureLocation) || string.IsNullOrWhiteSpace(arrivalLocation))
        {
            throw new ArgumentException("Vertreklocatie en aankomstlocatie zijn verplicht.");
        }

        if (type == TransportType.Vlucht &&
            (string.IsNullOrWhiteSpace(flightNumber) || string.IsNullOrWhiteSpace(airline)))
        {
            throw new ArgumentException(
                "Vluchtnummer en luchtvaartmaatschappij zijn verplicht bij type Vlucht.");
        }

        DayId = dayId;
        Type = type;
        DepartureLocation = departureLocation.Trim();
        ArrivalLocation = arrivalLocation.Trim();
        FlightNumber = string.IsNullOrWhiteSpace(flightNumber) ? null : flightNumber.Trim();
        Airline = string.IsNullOrWhiteSpace(airline) ? null : airline.Trim();
    }

    /// <summary>Hydratie van bestaand transport uit de database.</summary>
    public Transport(int id, int dayId, TransportType type, string departureLocation,
        string arrivalLocation, string? flightNumber, string? airline,
        DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        DayId = dayId;
        Type = type;
        DepartureLocation = departureLocation;
        ArrivalLocation = arrivalLocation;
        FlightNumber = flightNumber;
        Airline = airline;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}
