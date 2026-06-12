using System.Text.RegularExpressions;

namespace Logic.Models;

/// <summary>
/// Domeinmodel van de huisstijl van een reisbureau (FR-12). Bewaakt dat
/// kleuren geldige hex-codes zijn.
/// </summary>
public partial class Branding
{
    public int TravelAgentId { get; private set; }
    public string? LogoPath { get; private set; }
    public string PrimaryColor { get; private set; }
    public string AccentColor { get; private set; }

    public Branding(int travelAgentId, string? logoPath, string? primaryColor, string? accentColor)
    {
        TravelAgentId = travelAgentId;
        LogoPath = string.IsNullOrWhiteSpace(logoPath) ? null : logoPath.Trim();
        PrimaryColor = NormalizeColor(primaryColor, "#008fd1");
        AccentColor = NormalizeColor(accentColor, "#ff6800");
    }

    private static string NormalizeColor(string? color, string fallback)
    {
        if (string.IsNullOrWhiteSpace(color))
        {
            return fallback;
        }

        var trimmed = color.Trim();
        return HexColorRegex().IsMatch(trimmed)
            ? trimmed
            : throw new ArgumentException($"Ongeldige kleurcode: '{color}'. Gebruik hex, bv. #1d4ed8.");
    }

    [GeneratedRegex("^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6})$")]
    private static partial Regex HexColorRegex();
}
