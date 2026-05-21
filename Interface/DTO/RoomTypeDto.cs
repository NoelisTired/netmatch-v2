namespace Interface.DTO;

/// <summary>
/// Leesmodel voor een kamertype binnen een accommodatie (tabel <c>kamertype</c>).
/// </summary>
public record RoomTypeDto
{
    public int Id { get; init; }

    /// <summary>FK naar de accommodatie (accommodatie 1:N kamertype).</summary>
    public int AccommodationId { get; init; }

    public string Name { get; init; } = string.Empty;
    public decimal PricePerNight { get; init; }
    public int Capacity { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
