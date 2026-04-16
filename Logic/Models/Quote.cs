using System;

namespace Logic.Models;

public class Quote
{
    public int Id { get; private set; }
    public int TravelAgentId { get; private set; }
    public string Title { get; private set; }
    public string Language { get; private set; }
    public string Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; }
    public DateTime DeletedAt { get; private set; }

    public Quote(int id, int travelAgentId, string title, string language, string status)
    {
        this.Id = id;
        this.TravelAgentId = travelAgentId;
        this.Title = title;
        this.Language = language;
        this.Status = status;
    }
}
