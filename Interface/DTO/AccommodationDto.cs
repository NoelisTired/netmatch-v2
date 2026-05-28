namespace Interface.DTO;

/// <summary>
/// Leesmodel voor een accommodatie binnen een reisdag (tabel <c>accommodatie</c>).
/// </summary>
public record AccommodationDto
{
    public int Id { get; init; }

    /// <summary>FK naar de dag waar deze accommodatie bij hoort (dag 1:N accommodatie).</summary>
    public int DayId { get; init; }

    public string Name { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? Description { get; init; }
    public string? ImagePath { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
