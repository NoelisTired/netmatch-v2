using System;

namespace Logic.Models;

public class Contentblock
{
    public int Id { get; private set; }
    public string Title { get; set; }
    public string BodyText { get; private set; }
    public string Language { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; }
    public DateTime DeletedAt { get; private set; }

    public Contentblock(int id, string title, string bodyText, string language)
    {
        this.Id = id;
        this.Title = title;
        this.BodyText = bodyText;
        this.Language = language;
    }
}