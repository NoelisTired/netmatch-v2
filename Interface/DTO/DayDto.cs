namespace Interface.DTO;

/// <summary>
/// Leesmodel voor een reisdag binnen een offerte (tabel <c>dag</c>).
/// </summary>
public record DayDto
{
    public int Id { get; init; }

    /// <summary>FK naar de offerte waar deze dag bij hoort (offerte 1:N dag).</summary>
    public int QuoteId { get; init; }

    /// <summary>Dagnummer (INT, niet afgeleid van datum) zodat hernummeren kan.</summary>
    public int DayNumber { get; init; }

    public DateTime Date { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
