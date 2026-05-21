using Logic.Enums;

namespace Logic.Models;

/// <summary>
/// Domeinmodel van een offerte. Bewaakt zijn eigen invarianten: een offerte
/// zonder titel kan niet bestaan (FR-01). Losgekoppeld van DTO's en database.
/// </summary>
public class Quote
{
    public int Id { get; private set; }
    public int TravelAgentId { get; private set; }
    public string Title { get; private set; }
    public LanguageType Language { get; private set; }
    public StatusType Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    /// <summary>Nieuwe offerte (nog niet opgeslagen). Start standaard als concept.</summary>
    public Quote(int travelAgentId, string title, LanguageType language, StatusType status = StatusType.Concept)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Titel en taal zijn verplicht.", nameof(title));
        }

        TravelAgentId = travelAgentId;
        Title = title.Trim();
        Language = language;
        Status = status;
    }

    /// <summary>Hydratie van een bestaande offerte uit de database.</summary>
    public Quote(int id, int travelAgentId, string title, LanguageType language,
        StatusType status, DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        TravelAgentId = travelAgentId;
        Title = title;
        Language = language;
        Status = status;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}
