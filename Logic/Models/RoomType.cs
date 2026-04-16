using System;

namespace Logic.Models;

public class Kamertype
{
    public int Id { get; private set; }
    public int AccomodationId { get; private set; }
    public string Name { get; private set; }
    public decimal PricePerDay { get; private set; }
    public int MaxPersons { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; }
    public DateTime DeletedAt { get; private set; }

    public Kamertype(int id, int accomodationId, string name, decimal pricePerDay, int maxPersons)
    {
        this.Id = id;
        this.AccomodationId = accomodationId;
        this.Name = name;
        this.PricePerDay = pricePerDay;
        this.MaxPersons = maxPersons;
    }
}