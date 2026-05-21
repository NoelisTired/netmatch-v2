namespace Interface.DTO;

/// <summary>
/// Schrijfmodel voor het aanmaken/bijwerken van een offerte. Bevat geen
/// id of tijdstempels; die worden door de database / DAL beheerd.
/// </summary>
public record CreateQuoteDto
{
    public int TravelAgentId { get; init; }
    public string Title { get; init; } = string.Empty;

    /// <summary>Taalcode: NL, EN, DE of FR.</summary>
    public string Language { get; init; } = string.Empty;

    /// <summary>Status van de offerte, bv. "concept".</summary>
    public string Status { get; init; } = string.Empty;
}
