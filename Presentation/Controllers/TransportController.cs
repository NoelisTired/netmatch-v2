using Logic;
using Logic.Enums;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

/// <summary>
/// Beheert transportmiddelen binnen een reisdag (FR-03).
/// </summary>
public class TransportController : Controller
{
    private readonly TransportService _transportService;
    private readonly DayService _dayService;

    public TransportController(TransportService transportService, DayService dayService)
    {
        _transportService = transportService;
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
        return View(_transportService.GetTransportsForDay(dayId));
    }

    [HttpGet]
    public IActionResult Create(int dayId)
    {
        if (_dayService.GetDayById(dayId) is null)
        {
            return NotFound();
        }

        return View(new TransportViewModel { DayId = dayId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(TransportViewModel model)
    {
        var (error, _) = _transportService.AddTransport(model.DayId, model.Type,
            model.DepartureLocation, model.ArrivalLocation, model.FlightNumber, model.Airline);
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
        var transport = _transportService.GetTransportById(id);
        if (transport is null)
        {
            return NotFound();
        }

        return View(new TransportViewModel
        {
            Id = transport.Id,
            DayId = transport.DayId,
            Type = transport.Type.ToCode(),
            DepartureLocation = transport.DepartureLocation,
            ArrivalLocation = transport.ArrivalLocation,
            FlightNumber = transport.FlightNumber,
            Airline = transport.Airline
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(TransportViewModel model)
    {
        var error = _transportService.UpdateTransport(model.Id, model.Type,
            model.DepartureLocation, model.ArrivalLocation, model.FlightNumber, model.Airline);
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
        _transportService.DeleteTransport(id);
        return BackToQuote(dayId);
    }

    /// <summary>Terug naar de samengestelde offerte-pagina van deze dag.</summary>
    private IActionResult BackToQuote(int dayId)
    {
        var quoteId = _dayService.GetDayById(dayId)?.QuoteId ?? 0;
        return RedirectToAction("Details", "Quote", new { id = quoteId });
    }
}
