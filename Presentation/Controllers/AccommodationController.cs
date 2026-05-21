using Logic;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

/// <summary>
/// Beheert accommodaties binnen een reisdag (FR-04).
/// </summary>
public class AccommodationController : Controller
{
    private readonly AccommodationService _accommodationService;
    private readonly DayService _dayService;

    public AccommodationController(AccommodationService accommodationService, DayService dayService)
    {
        _accommodationService = accommodationService;
        _dayService = dayService;
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
        var (error, _) = _accommodationService.AddAccommodation(model.DayId, model.Name,
            model.Address, model.Description);
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
            Description = accommodation.Description
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(AccommodationViewModel model)
    {
        var error = _accommodationService.UpdateAccommodation(model.Id, model.Name,
            model.Address, model.Description);
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

    /// <summary>Terug naar de samengestelde offerte-pagina van deze dag.</summary>
    private IActionResult BackToQuote(int dayId)
    {
        var quoteId = _dayService.GetDayById(dayId)?.QuoteId ?? 0;
        return RedirectToAction("Details", "Quote", new { id = quoteId });
    }
}
