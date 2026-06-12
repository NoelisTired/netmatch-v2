using Microsoft.AspNetCore.Http;

namespace Presentation.Models;

/// <summary>
/// Formuliermodel voor de huisstijl-maker (FR-12). Kleurvalidatie leeft in
/// de Logic-laag; bestandstype/-grootte wordt in de controller bewaakt (TC-17).
/// </summary>
public class BrandingViewModel
{
    /// <summary>Huidig logo-pad (relatief t.o.v. wwwroot), voor weergave.</summary>
    public string? LogoPath { get; set; }

    /// <summary>Nieuw logo (optioneel; PNG/JPG/SVG, max 2 MB).</summary>
    public IFormFile? Logo { get; set; }

    public string PrimaryColor { get; set; } = "#008fd1";
    public string AccentColor { get; set; } = "#ff6800";
}
