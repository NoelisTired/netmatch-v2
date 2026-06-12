namespace Interface.DTO;

/// <summary>
/// Leesmodel voor een transportmiddel binnen een reisdag (tabel <c>transport</c>).
/// Transport = langere verplaatsing (vlucht/trein/bus/eigen vervoer).
/// </summary>
public record TransportDto
{
    public int Id { get; init; }

    /// <summary>FK naar de dag waar dit transport bij hoort (dag 1:N transport).</summary>
    public int DayId { get; init; }

    /// <summary>Transporttype: Vlucht, Bus, Trein of Eigen vervoer.</summary>
    public string Type { get; init; } = string.Empty;

    public string DepartureLocation { get; init; } = string.Empty;
    public string ArrivalLocation { get; init; } = string.Empty;

    /// <summary>Alleen gevuld bij type Vlucht.</summary>
    public string? FlightNumber { get; init; }

    /// <summary>Alleen gevuld bij type Vlucht.</summary>
    public string? Airline { get; init; }

    /// <summary>Indicatieve prijs in euro's.</summary>
    public decimal? Price { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
