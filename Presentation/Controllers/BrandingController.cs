using Logic;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

/// <summary>
/// Huisstijl-maker (FR-12): logo-upload + primaire/accentkleur per reisbureau.
/// </summary>
public class BrandingController : Controller
{
    // Zelfde demo-reisbureau als QuoteController tot er auth is.
    private const int DemoTravelAgentId = 1;

    private const long MaxLogoBytes = 2 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = { ".png", ".jpg", ".jpeg", ".svg" };
    private static readonly string[] AllowedContentTypes =
        { "image/png", "image/jpeg", "image/svg+xml" };

    private readonly BrandingService _brandingService;
    private readonly IWebHostEnvironment _environment;

    public BrandingController(BrandingService brandingService, IWebHostEnvironment environment)
    {
        _brandingService = brandingService;
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Edit()
    {
        var branding = _brandingService.GetForTravelAgent(DemoTravelAgentId);
        return View(new BrandingViewModel
        {
            LogoPath = branding.LogoPath,
            PrimaryColor = branding.PrimaryColor,
            AccentColor = branding.AccentColor
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(BrandingViewModel model)
    {
        var logoPath = _brandingService.GetForTravelAgent(DemoTravelAgentId).LogoPath;

        if (model.Logo is { Length: > 0 })
        {
            var extension = Path.GetExtension(model.Logo.FileName).ToLowerInvariant();

            if (model.Logo.Length > MaxLogoBytes ||
                !AllowedExtensions.Contains(extension) ||
                !AllowedContentTypes.Contains(model.Logo.ContentType))
            {
                ModelState.AddModelError(string.Empty,
                    "Alleen PNG, JPG en SVG zijn toegestaan. Maximale bestandsgrootte is 2 MB.");
                model.LogoPath = logoPath;
                return View(model);
            }

            logoPath = SaveLogo(model.Logo, extension);
        }

        var error = _brandingService.Save(DemoTravelAgentId, logoPath,
            model.PrimaryColor, model.AccentColor);
        if (error is not null)
        {
            ModelState.AddModelError(string.Empty, error);
            model.LogoPath = logoPath;
            return View(model);
        }

        TempData["Toast"] = "Huisstijl opgeslagen.";
        return RedirectToAction(nameof(Edit));
    }

    private string SaveLogo(IFormFile logo, string extension)
    {
        var folder = Path.Combine(_environment.WebRootPath, "uploads", "branding");
        Directory.CreateDirectory(folder);

        var fileName = $"agent_{DemoTravelAgentId}{extension}";
        var fullPath = Path.Combine(folder, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            logo.CopyTo(stream);
        }

        return $"/uploads/branding/{fileName}";
    }
}
