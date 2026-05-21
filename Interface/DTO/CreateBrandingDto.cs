namespace Interface.DTO;

/// <summary>
/// Schrijfmodel voor het instellen/bijwerken van de huisstijl van een
/// reisbureau (INSERT of UPDATE; 1:1 met travelagent).
/// </summary>
public record CreateBrandingDto
{
    public int TravelAgentId { get; init; }
    public string? LogoPath { get; init; }
    public string PrimaryColor { get; init; } = "#0d6efd";
    public string AccentColor { get; init; } = "#6c757d";
}
