using System.Diagnostics;
using System.Globalization;
using Logic;
using Logic.Enums;
using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace Presentation.Controllers;

public class HomeController : Controller
{
    // Tot er login/auth is, hangen offertes aan een vast reisbureau (zie QuoteController).
    private const int DemoTravelAgentId = 1;
    private const int RecentQuoteCount = 5;
    private const int ChartMonthCount = 6;

    private readonly QuoteService _quoteService;

    public HomeController(QuoteService quoteService)
    {
        _quoteService = quoteService;
    }

    public IActionResult Index()
    {
        var quotes = _quoteService.GetQuotesForTravelAgent(DemoTravelAgentId).ToList();

        var model = new DashboardViewModel
        {
            TotalQuotes = quotes.Count,
            DraftCount = quotes.Count(q => q.Status == StatusType.Concept),
            FinalCount = quotes.Count(q => q.Status == StatusType.Definitief),
            RecentQuotes = quotes.OrderByDescending(q => q.CreatedAt).Take(RecentQuoteCount).ToList(),
            QuotesPerMonth = BuildMonthlyCounts(quotes)
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /// <summary>Aantal offertes per maand voor de laatste maanden, oudste eerst.</summary>
    private static List<MonthlyQuoteCount> BuildMonthlyCounts(IReadOnlyCollection<Logic.Models.Quote> quotes)
    {
        var culture = CultureInfo.CurrentUICulture;
        var thisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        return Enumerable.Range(0, ChartMonthCount)
            .Select(offset => thisMonth.AddMonths(offset - (ChartMonthCount - 1)))
            .Select(month => new MonthlyQuoteCount
            {
                Label = month.ToString("MMM", culture),
                Count = quotes.Count(q => q.CreatedAt.Year == month.Year && q.CreatedAt.Month == month.Month)
            })
            .ToList();
    }
}
