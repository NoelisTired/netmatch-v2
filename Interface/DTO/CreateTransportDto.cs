namespace Interface.DTO;

/// <summary>
/// Schrijfmodel voor het toevoegen/bijwerken van een transportmiddel.
/// </summary>
public record CreateTransportDto
{
    public int DayId { get; init; }
    public string Type { get; init; } = string.Empty;
    public string DepartureLocation { get; init; } = string.Empty;
    public string ArrivalLocation { get; init; } = string.Empty;
    public string? FlightNumber { get; init; }
    public string? Airline { get; init; }
    public decimal? Price { get; init; }
}
