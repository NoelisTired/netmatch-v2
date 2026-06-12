namespace Presentation.Models;

/// <summary>
/// Formuliermodel voor het toevoegen/bewerken van transport. Validatie
/// (FR-03) leeft in de Logic-laag.
/// </summary>
public class TransportViewModel
{
    public int Id { get; set; }

    /// <summary>Reisdag waar dit transport bij hoort.</summary>
    public int DayId { get; set; }

    public string? Type { get; set; }
    public string? DepartureLocation { get; set; }
    public string? ArrivalLocation { get; set; }
    public string? FlightNumber { get; set; }
    public string? Airline { get; set; }
    public decimal? Price { get; set; }
}
