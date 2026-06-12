using Logic;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

/// <summary>
/// Beheert kamertypes binnen een accommodatie (FR-04).
/// </summary>
public class RoomTypeController : Controller
{
    private readonly RoomTypeService _roomTypeService;
    private readonly AccommodationService _accommodationService;
    private readonly DayService _dayService;

    public RoomTypeController(RoomTypeService roomTypeService,
        AccommodationService accommodationService, DayService dayService)
    {
        _roomTypeService = roomTypeService;
        _accommodationService = accommodationService;
        _dayService = dayService;
    }

    public IActionResult Index(int accommodationId)
    {
        var accommodation = _accommodationService.GetAccommodationById(accommodationId);
        if (accommodation is null)
        {
            return NotFound();
        }

        ViewBag.AccommodationId = accommodationId;
        ViewBag.DayId = accommodation.DayId;
        ViewBag.AccommodationName = accommodation.Name;
        return View(_roomTypeService.GetRoomTypesForAccommodation(accommodationId));
    }

    [HttpGet]
    public IActionResult Create(int accommodationId)
    {
        if (_accommodationService.GetAccommodationById(accommodationId) is null)
        {
            return NotFound();
        }

        return View(new RoomTypeViewModel { AccommodationId = accommodationId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(RoomTypeViewModel model)
    {
        var (error, _) = _roomTypeService.AddRoomType(model.AccommodationId, model.Name,
            model.PricePerNight, model.Capacity);
        if (error is not null)
        {
            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        TempData["Toast"] = "Kamertype toegevoegd.";
        TempData["Toast"] = "Kamertype opgeslagen.";
        return BackToQuote(model.AccommodationId);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var roomType = _roomTypeService.GetRoomTypeById(id);
        if (roomType is null)
        {
            return NotFound();
        }

        return View(new RoomTypeViewModel
        {
            Id = roomType.Id,
            AccommodationId = roomType.AccommodationId,
            Name = roomType.Name,
            PricePerNight = roomType.PricePerNight,
            Capacity = roomType.Capacity
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(RoomTypeViewModel model)
    {
        var error = _roomTypeService.UpdateRoomType(model.Id, model.Name,
            model.PricePerNight, model.Capacity);
        if (error is not null)
        {
            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        return BackToQuote(model.AccommodationId);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id, int accommodationId)
    {
        _roomTypeService.DeleteRoomType(id);
        TempData["Toast"] = "Kamertype verwijderd.";
        return BackToQuote(accommodationId);
    }

    /// <summary>Terug naar de samengestelde offerte-pagina via accommodatie → dag.</summary>
    private IActionResult BackToQuote(int accommodationId)
    {
        var accommodation = _accommodationService.GetAccommodationById(accommodationId);
        var quoteId = accommodation is null
            ? 0
            : _dayService.GetDayById(accommodation.DayId)?.QuoteId ?? 0;
        return RedirectToAction("Details", "Quote", new { id = quoteId });
    }
}
