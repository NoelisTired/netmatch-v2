namespace Interface.DTO;

/// <summary>
/// Schrijfmodel voor het toevoegen/bijwerken van een kamertype.
/// </summary>
public record CreateRoomTypeDto
{
    public int AccommodationId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal PricePerNight { get; init; }
    public int Capacity { get; init; }
}
