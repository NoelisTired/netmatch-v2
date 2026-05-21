namespace Presentation.Models;

/// <summary>
/// Formuliermodel voor het toevoegen/bewerken van een reisdag. Validatie
/// (FR-02) leeft in de Logic-laag.
/// </summary>
public class DayViewModel
{
    public int Id { get; set; }

    /// <summary>Offerte waar deze dag bij hoort.</summary>
    public int QuoteId { get; set; }

    public int? DayNumber { get; set; }

    public DateTime? Date { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }
}
