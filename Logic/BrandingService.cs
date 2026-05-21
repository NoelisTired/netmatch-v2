using Interface.DataInterfaces;
using Interface.DTO;
using Logic.Models;

namespace Logic;

/// <summary>
/// Bedrijfslogica rond de huisstijl (FR-12). Geeft altijd een Branding terug
/// (met standaardwaarden als die nog niet is ingesteld) en valideert kleuren
/// bij het opslaan.
/// </summary>
public class BrandingService
{
    private readonly IBrandingRepository _brandingRepository;

    public BrandingService(IBrandingRepository brandingRepository)
    {
        _brandingRepository = brandingRepository;
    }

    /// <summary>Huisstijl van het reisbureau; standaardwaarden als nog niet ingesteld.</summary>
    public Branding GetForTravelAgent(int travelAgentId)
    {
        var dto = _brandingRepository.GetByTravelAgent(travelAgentId);
        return dto is null
            ? new Branding(travelAgentId, null, null, null)
            : new Branding(dto.TravelAgentId, dto.LogoPath, dto.PrimaryColor, dto.AccentColor);
    }

    /// <summary>
    /// Slaat de huisstijl op. Kleuren moeten geldige hex zijn; het
    /// logo-bestand wordt door de presentatielaag opgeslagen en hier alleen
    /// als pad doorgegeven.
    /// </summary>
    public string? Save(int travelAgentId, string? logoPath, string? primaryColor, string? accentColor)
    {
        try
        {
            var branding = new Branding(travelAgentId, logoPath, primaryColor, accentColor);

            _brandingRepository.Save(new CreateBrandingDto
            {
                TravelAgentId = branding.TravelAgentId,
                LogoPath = branding.LogoPath,
                PrimaryColor = branding.PrimaryColor,
                AccentColor = branding.AccentColor
            });

            return null;
        }
        catch (ArgumentException ex)
        {
            return ex.Message;
        }
    }
}
