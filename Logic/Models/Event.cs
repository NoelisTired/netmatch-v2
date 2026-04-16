using System;

namespace Logic.Models;

public class Event
{
    public int Id { get; private set; }
    public int DagId { get; private set; }
    public string Name { get; private set; }
    public decimal PricePerNight { get; private set; }
    public int MaxPersons { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; }
    public DateTime DeletedAt { get; private set; }


    public Event(int id, int dagId, string name, decimal pricePerNight, int maxPersons)
    {
        this.Id = id;
        this.DagId = dagId;
        this.Name = name;
        this.PricePerNight = pricePerNight;
        this.MaxPersons = maxPersons;
    }
}
