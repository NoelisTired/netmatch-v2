namespace Interface.DTO;

/// <summary>
/// Schrijfmodel voor het toevoegen/bijwerken van een accommodatie.
/// </summary>
public record CreateAccommodationDto
{
    public int DayId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Address { get; init; }
    public string? Description { get; init; }
}
