namespace Interface.DTO;

/// <summary>
/// Leesmodel voor een offerte zoals die uit de database komt (tabel <c>offerte</c>).
/// </summary>
public record QuoteDto
{
    public int Id { get; init; }
    public int TravelAgentId { get; init; }
    public string Title { get; init; } = string.Empty;

    /// <summary>Taalcode: NL, EN, DE of FR (NVARCHAR-kolom <c>taal</c>).</summary>
    public string Language { get; init; } = string.Empty;

    /// <summary>Status van de offerte, bv. "concept" of "definitief".</summary>
    public string Status { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
