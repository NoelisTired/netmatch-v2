namespace Interface.DTO;

/// <summary>
/// Schrijfmodel voor het instellen/bijwerken van de huisstijl van een
/// reisbureau (INSERT of UPDATE; 1:1 met travelagent).
/// </summary>
public record CreateBrandingDto
{
    public int TravelAgentId { get; init; }
    public string? LogoPath { get; init; }
    public string PrimaryColor { get; init; } = "#008fd1";
    public string AccentColor { get; init; } = "#ff6800";
}
