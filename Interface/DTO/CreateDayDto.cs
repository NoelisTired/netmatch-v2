namespace Interface.DTO;

/// <summary>
/// Schrijfmodel voor het toevoegen/bijwerken van een reisdag.
/// </summary>
public record CreateDayDto
{
    public int QuoteId { get; init; }
    public int DayNumber { get; init; }
    public DateTime Date { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
}
