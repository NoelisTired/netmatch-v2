namespace Presentation.Models;

/// <summary>
/// Formuliermodel voor het toevoegen/bewerken van een kamertype.
/// Validatie (FR-04) leeft in de Logic-laag.
/// </summary>
public class RoomTypeViewModel
{
    public int Id { get; set; }

    /// <summary>Accommodatie waar dit kamertype bij hoort.</summary>
    public int AccommodationId { get; set; }

    public string? Name { get; set; }
    public decimal? PricePerNight { get; set; }
    public int Capacity { get; set; } = 2;
}
