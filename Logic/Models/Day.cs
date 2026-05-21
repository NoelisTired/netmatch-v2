namespace Logic.Models;

/// <summary>
/// Domeinmodel van een reisdag binnen een offerte (FR-02). Een dag kan niet
/// bestaan zonder dagnummer en datum; die invariant bewaakt dit model zelf.
/// </summary>
public class Day
{
    public int Id { get; private set; }
    public int QuoteId { get; private set; }
    public int DayNumber { get; private set; }
    public DateTime Date { get; private set; }
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    /// <summary>Nieuwe dag (nog niet opgeslagen).</summary>
    public Day(int quoteId, int dayNumber, DateTime date, string? title = null, string? description = null)
    {
        if (dayNumber <= 0)
        {
            throw new ArgumentException("Dagnummer en datum zijn verplicht.", nameof(dayNumber));
        }

        QuoteId = quoteId;
        DayNumber = dayNumber;
        Date = date;
        Title = string.IsNullOrWhiteSpace(title) ? null : title.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }

    /// <summary>Hydratie van een bestaande dag uit de database.</summary>
    public Day(int id, int quoteId, int dayNumber, DateTime date, string? title,
        string? description, DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        QuoteId = quoteId;
        DayNumber = dayNumber;
        Date = date;
        Title = title;
        Description = description;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}
