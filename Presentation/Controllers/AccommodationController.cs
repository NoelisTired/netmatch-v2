using Logic;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

/// <summary>
/// Beheert accommodaties binnen een reisdag (FR-04).
/// </summary>
public class AccommodationController : Controller
{
    private const long MaxImageBytes = 5 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = { ".png", ".jpg", ".jpeg", ".webp" };
    private static readonly string[] AllowedContentTypes =
        { "image/png", "image/jpeg", "image/webp" };

    private readonly AccommodationService _accommodationService;
    private readonly DayService _dayService;
    private readonly IWebHostEnvironment _environment;

    public AccommodationController(AccommodationService accommodationService, DayService dayService,
        IWebHostEnvironment environment)
    {
        _accommodationService = accommodationService;
        _dayService = dayService;
        _environment = environment;
    }

    public IActionResult Index(int dayId)
    {
        var day = _dayService.GetDayById(dayId);
        if (day is null)
        {
            return NotFound();
        }

        ViewBag.DayId = dayId;
        ViewBag.QuoteId = day.QuoteId;
        ViewBag.DayNumber = day.DayNumber;
        return View(_accommodationService.GetAccommodationsForDay(dayId));
    }

    [HttpGet]
    public IActionResult Create(int dayId)
    {
        if (_dayService.GetDayById(dayId) is null)
        {
            return NotFound();
        }

        return View(new AccommodationViewModel { DayId = dayId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(AccommodationViewModel model)
    {
        var imagePath = SaveImage(model.Image);
        if (model.Image is { Length: > 0 } && imagePath is null)
        {
            return View(model);
        }

        var (error, _) = _accommodationService.AddAccommodation(model.DayId, model.Name,
            model.Address, model.Description, imagePath);
        if (error is not null)
        {
            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        return BackToQuote(model.DayId);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var accommodation = _accommodationService.GetAccommodationById(id);
        if (accommodation is null)
        {
            return NotFound();
        }

        return View(new AccommodationViewModel
        {
            Id = accommodation.Id,
            DayId = accommodation.DayId,
            Name = accommodation.Name,
            Address = accommodation.Address,
            Description = accommodation.Description,
            ImagePath = accommodation.ImagePath
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(AccommodationViewModel model)
    {
        var imagePath = model.ImagePath;

        if (model.Image is { Length: > 0 })
        {
            var newPath = SaveImage(model.Image);
            if (newPath is null)
            {
                return View(model);
            }

            imagePath = newPath;
        }

        var error = _accommodationService.UpdateAccommodation(model.Id, model.Name,
            model.Address, model.Description, imagePath);
        if (error is not null)
        {
            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        return BackToQuote(model.DayId);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id, int dayId)
    {
        _accommodationService.DeleteAccommodation(id);
        return BackToQuote(dayId);
    }

    /// <summary>Slaat een geüpload plaatje op en geeft het relatieve webpad terug.</summary>
    private string? SaveImage(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        if (file.Length > MaxImageBytes)
        {
            ModelState.AddModelError("Image", "Afbeelding mag maximaal 5 MB zijn.");
            return null;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension) || !AllowedContentTypes.Contains(file.ContentType))
        {
            ModelState.AddModelError("Image", "Alleen PNG, JPG of WebP afbeeldingen zijn toegestaan.");
            return null;
        }

        var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads", "accommodations");
        Directory.CreateDirectory(uploadsDir);

        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsDir, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        file.CopyTo(stream);

        return $"/uploads/accommodations/{fileName}";
    }

    /// <summary>Terug naar de samengestelde offerte-pagina van deze dag.</summary>
    private IActionResult BackToQuote(int dayId)
    {
        var quoteId = _dayService.GetDayById(dayId)?.QuoteId ?? 0;
        return RedirectToAction("Details", "Quote", new { id = quoteId });
    }
}
