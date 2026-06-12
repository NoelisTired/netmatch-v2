using Logic;
using Logic.Enums;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;
using Presentation.Pdf;
using QuestPDF.Fluent;

namespace Presentation.Controllers;

public class QuoteController : Controller
{
    // Tot er login/auth is, hangen offertes aan één vast reisbureau.
    // FR-09 (data-isolatie per TravelAgent) komt met de auth-feature.
    private const int DemoTravelAgentId = 1;

    private readonly QuoteService _quoteService;
    private readonly DayService _dayService;
    private readonly TransportService _transportService;
    private readonly AccommodationService _accommodationService;
    private readonly RoomTypeService _roomTypeService;
    private readonly BrandingService _brandingService;
    private readonly IWebHostEnvironment _environment;

    public QuoteController(
        QuoteService quoteService,
        DayService dayService,
        TransportService transportService,
        AccommodationService accommodationService,
        RoomTypeService roomTypeService,
        BrandingService brandingService,
        IWebHostEnvironment environment)
    {
        _quoteService = quoteService;
        _dayService = dayService;
        _transportService = transportService;
        _accommodationService = accommodationService;
        _roomTypeService = roomTypeService;
        _brandingService = brandingService;
        _environment = environment;
    }

    public IActionResult Index()
    {
        var quotes = _quoteService.GetQuotesForTravelAgent(DemoTravelAgentId);
        return View(quotes);
    }

    /// <summary>
    /// Volledige offerte op één pagina: dagen → transport/accommodatie →
    /// kamertypes, in één samengesteld model.
    /// </summary>
    public IActionResult Details(int id)
    {
        var overview = BuildOverview(id);
        return overview is null ? NotFound() : View(overview);
    }

    /// <summary>FR-11: genereert de offerte als PDF met de huisstijl.</summary>
    public IActionResult Pdf(int id)
    {
        var overview = BuildOverview(id);
        if (overview is null)
        {
            return NotFound();
        }

        var bytes = new QuotePdfDocument(overview, _environment.WebRootPath).GeneratePdf();
        return File(bytes, "application/pdf", $"offerte-{id}.pdf");
    }

    /// <summary>
    /// Stelt de volledige offerte samen: dagen → transport/accommodatie →
    /// kamertypes + huisstijl. Hergebruikt door Details (HTML) en Pdf.
    /// </summary>
    private QuoteOverviewViewModel? BuildOverview(int id)
    {
        var quote = _quoteService.GetQuoteById(id);
        if (quote is null)
        {
            return null;
        }

        var overview = new QuoteOverviewViewModel
        {
            Quote = quote,
            Branding = _brandingService.GetForTravelAgent(DemoTravelAgentId)
        };

        foreach (var day in _dayService.GetDaysForQuote(quote.Id))
        {
            var transports = _transportService.GetTransportsForDay(day.Id).ToList();
            overview.IndicativeTransportTotal += transports
                .Where(t => t.Price.HasValue).Sum(t => t.Price!.Value);

            var dayBlock = new DayBlock
            {
                Day = day,
                Transports = transports
            };

            foreach (var accommodation in _accommodationService.GetAccommodationsForDay(day.Id))
            {
                var roomTypes = _roomTypeService
                    .GetRoomTypesForAccommodation(accommodation.Id).ToList();

                if (roomTypes.Count > 0)
                {
                    overview.IndicativeAccommodationTotal += roomTypes.Min(r => r.PricePerNight);
                }

                dayBlock.Accommodations.Add(new AccommodationBlock
                {
                    Accommodation = accommodation,
                    RoomTypes = roomTypes
                });
            }

            overview.Days.Add(dayBlock);
        }

        return overview;
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new QuoteViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(QuoteViewModel model)
    {
        var (error, id) = _quoteService.CreateQuote(DemoTravelAgentId, model.Title, model.Language);
        if (error is not null)
        {
            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        TempData["Toast"] = "Offerte aangemaakt.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var quote = _quoteService.GetQuoteById(id);
        if (quote is null)
        {
            return NotFound();
        }

        return View(new QuoteViewModel
        {
            Id = quote.Id,
            Title = quote.Title,
            Language = quote.Language.ToCode(),
            Status = quote.Status.ToCode()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(QuoteViewModel model)
    {
        var error = _quoteService.UpdateQuote(model.Id, model.Title, model.Language, model.Status);
        if (error is not null)
        {
            ModelState.AddModelError(string.Empty, error);
            return View(model);
        }

        TempData["Toast"] = "Offerte opgeslagen.";
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        _quoteService.DeleteQuote(id);
        TempData["Toast"] = "Offerte verwijderd.";
        return RedirectToAction(nameof(Index));
    }
}
