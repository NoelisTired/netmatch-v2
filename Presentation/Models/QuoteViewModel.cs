namespace Presentation.Models;

/// <summary>
/// Formuliermodel voor het aanmaken/bewerken van een offerte. De
/// validatieregels (FR-01) leven in de Logic-laag; deze klasse draagt
/// alleen de ruwe invoer over.
/// </summary>
public class QuoteViewModel
{
    public int Id { get; set; }

    public string? Title { get; set; }

    /// <summary>Taalcode: NL, EN, DE of FR.</summary>
    public string? Language { get; set; }

    /// <summary>Status: "concept" of "definitief".</summary>
    public string? Status { get; set; }
}
