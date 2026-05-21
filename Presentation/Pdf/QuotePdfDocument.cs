using Presentation.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Presentation.Pdf;

/// <summary>
/// FR-11: rendert een volledige offerte als PDF, met de huisstijl (logo +
/// primaire/accentkleur) van het reisbureau (FR-12).
/// </summary>
public class QuotePdfDocument : IDocument
{
    private readonly QuoteOverviewViewModel _model;
    private readonly string _webRootPath;
    private readonly Color _primary;
    private readonly Color _accent;

    public QuotePdfDocument(QuoteOverviewViewModel model, string webRootPath)
    {
        _model = model;
        _webRootPath = webRootPath;
        _primary = ParseColor(model.Branding.PrimaryColor, Colors.Blue.Medium);
        _accent = ParseColor(model.Branding.AccentColor, Colors.Grey.Darken1);
    }

    private static Color ParseColor(string? hex, Color fallback)
    {
        if (string.IsNullOrWhiteSpace(hex))
        {
            return fallback;
        }

        try
        {
            return Color.FromHex(hex);
        }
        catch
        {
            return fallback;
        }
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(2, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(10));

            page.Header().Element(ComposeHeader);
            page.Content().PaddingVertical(12).Element(ComposeContent);
            page.Footer().AlignCenter().Text(text =>
            {
                text.Span("Pagina ");
                text.CurrentPageNumber();
                text.Span(" / ");
                text.TotalPages();
            });
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.BorderBottom(2).BorderColor(_primary).PaddingBottom(8).Row(row =>
        {
            var logo = ResolveLogoPath();
            if (logo is not null)
            {
                row.ConstantItem(90).Height(48).AlignLeft().AlignMiddle().Image(logo).FitArea();
            }

            row.RelativeItem().Column(col =>
            {
                col.Item().Text(_model.Quote.Title).FontSize(20).Bold().FontColor(_primary);
                col.Item().Text($"Taal: {_model.Quote.Language} · Status: {_model.Quote.Status} · " +
                                $"Aangemaakt {_model.Quote.CreatedAt:dd-MM-yyyy}")
                    .FontSize(9).FontColor(Colors.Grey.Darken1);
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(14);

            if (_model.Days.Count == 0)
            {
                column.Item().Text("Deze offerte bevat nog geen dagen.").Italic();
            }

            foreach (var block in _model.Days)
            {
                column.Item().Column(day =>
                {
                    var title = string.IsNullOrWhiteSpace(block.Day.Title)
                        ? $"Dag {block.Day.DayNumber}"
                        : $"Dag {block.Day.DayNumber} – {block.Day.Title}";

                    day.Item().Background(_primary).Padding(6)
                        .Text($"{title}  ({block.Day.Date:dd-MM-yyyy})")
                        .FontColor(Colors.White).Bold();

                    if (!string.IsNullOrWhiteSpace(block.Day.Description))
                    {
                        day.Item().PaddingTop(4).Text(block.Day.Description!)
                            .FontColor(Colors.Grey.Darken2);
                    }

                    if (block.Transports.Count > 0)
                    {
                        day.Item().PaddingTop(6).Text("Transport").Bold().FontColor(_accent);
                        foreach (var t in block.Transports)
                        {
                            var flight = t.FlightNumber is null ? "" : $" ({t.FlightNumber} · {t.Airline})";
                            day.Item().Text(
                                $"• {t.Type}: {t.DepartureLocation} → {t.ArrivalLocation}{flight}");
                        }
                    }

                    foreach (var acc in block.Accommodations)
                    {
                        day.Item().PaddingTop(6)
                            .Text($"Accommodatie: {acc.Accommodation.Name}").Bold().FontColor(_accent);
                        if (!string.IsNullOrWhiteSpace(acc.Accommodation.Address))
                        {
                            day.Item().Text(acc.Accommodation.Address!).FontSize(9)
                                .FontColor(Colors.Grey.Darken1);
                        }

                        if (acc.RoomTypes.Count > 0)
                        {
                            day.Item().PaddingTop(2).Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn(3);
                                    c.RelativeColumn();
                                    c.RelativeColumn();
                                });
                                foreach (var rt in acc.RoomTypes)
                                {
                                    table.Cell().Text(rt.Name);
                                    table.Cell().Text($"€ {rt.PricePerNight:0.00}/nacht");
                                    table.Cell().Text($"{rt.Capacity} p.");
                                }
                            });
                        }
                    }
                });
            }

            if (_model.IndicativeAccommodationTotal > 0)
            {
                column.Item().PaddingTop(10).AlignRight().Text(
                        $"Indicatie accommodatie: € {_model.IndicativeAccommodationTotal:0.00} per nacht")
                    .Bold().FontColor(_primary);
            }
        });
    }

    /// <summary>Fysiek pad naar het logo, of null als er geen (raster)logo is.</summary>
    private string? ResolveLogoPath()
    {
        var logoPath = _model.Branding.LogoPath;
        if (string.IsNullOrWhiteSpace(logoPath))
        {
            return null;
        }

        // QuestPDF .Image() ondersteunt rasterformaten; SVG slaan we over.
        if (logoPath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var relative = logoPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var full = Path.Combine(_webRootPath, relative);
        return File.Exists(full) ? full : null;
    }
}
