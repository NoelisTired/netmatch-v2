using Logic.Models;

namespace Presentation.Models;

/// <summary>
/// Samenvatting voor het dashboard: kerncijfers, recente offertes en het
/// aantal aangemaakte offertes per maand (voor de staafgrafiek).
/// </summary>
public class DashboardViewModel
{
    public int TotalQuotes { get; set; }
    public int DraftCount { get; set; }
    public int FinalCount { get; set; }

    public List<Quote> RecentQuotes { get; set; } = new();

    public List<MonthlyQuoteCount> QuotesPerMonth { get; set; } = new();
}

/// <summary>Aantal offertes in een kalendermaand, met gelokaliseerd label.</summary>
public class MonthlyQuoteCount
{
    public string Label { get; set; } = string.Empty;
    public int Count { get; set; }
}
