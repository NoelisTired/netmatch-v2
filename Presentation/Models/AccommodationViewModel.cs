namespace Presentation.Models;

/// <summary>
/// Formuliermodel voor het toevoegen/bewerken van een accommodatie.
/// Validatie (FR-04) leeft in de Logic-laag.
/// </summary>
public class AccommodationViewModel
{
    public int Id { get; set; }

    /// <summary>Reisdag waar deze accommodatie bij hoort.</summary>
    public int DayId { get; set; }

    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
}
