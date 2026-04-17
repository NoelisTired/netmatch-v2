using System;
using Logic.Enums;

namespace Logic.Models;

public class Quote
{
    public int Id { get; private set; }
    public int TravelAgentId { get; private set; }
    public string Title { get; private set; }
    public LanguageType Language { get; private set; }
    public StatusType Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; }
    public DateTime DeletedAt { get; private set; }

    public Quote(int id, int travelAgentId, string title, LanguageType language, StatusType status)
    {
        this.Id = id;
        this.TravelAgentId = travelAgentId;
        this.Title = title ?? throw new ArgumentNullException(nameof(title));
        this.Language = language;
        this.Status = status;
    }
}
