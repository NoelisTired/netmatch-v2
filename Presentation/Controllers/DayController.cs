using Logic;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

/// <summary>
/// Beheert het dag-tot-dag overzicht binnen een offerte (FR-02).
/// </summary>
public class DayController : Controller
{
    private readonly DayService _dayService;
    private readonly QuoteService _quoteService;

    public DayController(DayService dayService, QuoteService quoteService)
    {
        _dayService = dayService;
        _quoteService = quoteService;
    }

    public IActionResult Index(int quoteId)
    {
        var quote = _quoteService.GetQuoteById(quoteId);
        if (quote is null)
        {
            return NotFound();
        }

        ViewBag.QuoteId = quoteId;
        ViewBag.QuoteTitle = quote.Title;
        return View(_dayService.GetDaysForQuote(quoteId));
    }

    [HttpGet]
    public IActionResult Create(int quoteId)
    {
        if (_quoteService.GetQuoteById(quoteId) is null)
        {
            return NotFound();
        }

        return View(new DayViewModel { QuoteId = quoteId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(DayViewModel model)
    {
        var (error, _) = _dayService.AddDay(model.QuoteId, model.DayNumber, model.Date,
            model.Title, model.Description);
        if (error is not null)
        {
            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        return RedirectToAction("Details", "Quote", new { id = model.QuoteId });
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var day = _dayService.GetDayById(id);
        if (day is null)
        {
            return NotFound();
        }

        return View(new DayViewModel
        {
            Id = day.Id,
            QuoteId = day.QuoteId,
            DayNumber = day.DayNumber,
            Date = day.Date,
            Title = day.Title,
            Description = day.Description
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(DayViewModel model)
    {
        var error = _dayService.UpdateDay(model.Id, model.DayNumber, model.Date,
            model.Title, model.Description);
        if (error is not null)
        {
            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        return RedirectToAction("Details", "Quote", new { id = model.QuoteId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id, int quoteId)
    {
        _dayService.DeleteDay(id);
        return RedirectToAction("Details", "Quote", new { id = quoteId });
    }
}
