using System;

namespace Logic.Models;

public class DayToDay
{
    public int Id { get; }
    public int DayNumber { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime Date { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime Delete_At { get; private set; }




    // je moet laten zien hoe waarden worden gezet. (aka Constructors)
    public DayToDay(int id, int dayNumber, string title, string description, DateTime date)
    {
        this.Id = id;
        this.DayNumber = dayNumber;
        this.Title = title;
        this.Description = description;
        this.Date = date;
    }
    // update / bewerken van de beschrijving
    public void UpdateDescription(string description)
    {
        Description = description;
    }
}

