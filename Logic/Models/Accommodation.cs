namespace Logic.Models;

/// <summary>
/// Domeinmodel van een accommodatie binnen een reisdag (FR-04). Een
/// accommodatie kan niet bestaan zonder naam (TC-08).
/// </summary>
public class Accommodation
{
    public int Id { get; private set; }
    public int DayId { get; private set; }
    public string Name { get; private set; }
    public string? Address { get; private set; }
    public string? Description { get; private set; }
    public string? ImagePath { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    /// <summary>Nieuwe accommodatie (nog niet opgeslagen).</summary>
    public Accommodation(int dayId, string? name, string? address = null, string? description = null,
        string? imagePath = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Naam van de accommodatie is verplicht.", nameof(name));
        }

        DayId = dayId;
        Name = name.Trim();
        Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        ImagePath = imagePath;
    }

    /// <summary>Hydratie van een bestaande accommodatie uit de database.</summary>
    public Accommodation(int id, int dayId, string name, string? address, string? description,
        string? imagePath, DateTime createdAt, DateTime updatedAt)
    {
        Id = id;
        DayId = dayId;
        Name = name;
        Address = address;
        Description = description;
        ImagePath = imagePath;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }
}
