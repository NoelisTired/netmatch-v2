namespace Logic.Models;

/// <summary>
/// Domeinmodel van een kamertype binnen een accommodatie (FR-04). Naam en
/// prijs per nacht zijn verplicht (TC-09).
/// </summary>
public class RoomType
{
    public int Id { get; private set; }
    public int AccommodationId { get; private set; }
    public string Name { get; private set; }
    public decimal PricePerNight { get; private set; }
    public int Capacity { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    /// <summary>Nieuw kamertype (nog niet opgeslagen).</summary>
    public RoomType(int accommodationId, string? name, decimal? pricePerNight, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name) || pricePerNight is null or <= 0)
        {
            throw new ArgumentException(
                "Naam en prijs per nacht zijn verplicht voor een kamertype.");
        }

        AccommodationId = accommodationId;
        Name = name.Trim();
        PricePerNight = pricePerNight.Value;
        Capacity = capacity < 1 ? 1 : capacity;
    }

    /// <summary>Hydratie van een bestaand kamertype uit de database.</summary>
    public RoomType(int id, int accommodationId, string name, decimal pricePerNight,
        int capacity, DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        AccommodationId = accommodationId;
        Name = name;
        PricePerNight = pricePerNight;
        Capacity = capacity;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}
