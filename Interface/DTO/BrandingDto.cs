namespace Interface.DTO;

/// <summary>
/// Leesmodel voor de huisstijl van een reisbureau (tabel <c>branding</c>,
/// 1:1 met travelagent).
/// </summary>
public record BrandingDto
{
    public int Id { get; init; }
    public int TravelAgentId { get; init; }

    /// <summary>Pad naar het geüploade logo (relatief t.o.v. wwwroot), of null.</summary>
    public string? LogoPath { get; init; }

    /// <summary>Primaire huisstijlkleur als hex, bv. <c>#1d4ed8</c>.</summary>
    public string PrimaryColor { get; init; } = "#0d6efd";

    /// <summary>Accentkleur als hex.</summary>
    public string AccentColor { get; init; } = "#6c757d";

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
