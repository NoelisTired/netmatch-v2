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
    private readonly Color _lightBg;

    public QuotePdfDocument(QuoteOverviewViewModel model, string webRootPath)
    {
        _model = model;
        _webRootPath = webRootPath;
        _primary = ParseColor(model.Branding.PrimaryColor, Colors.Blue.Medium);
        _accent = ParseColor(model.Branding.AccentColor, Colors.Grey.Darken1);
        _lightBg = Color.FromHex("#F5F6F8");
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
            page.Margin(1.8f, Unit.Centimetre);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(9.5f).FontColor(Colors.Grey.Darken3));

            page.Header().Element(ComposeHeader);
            page.Content().PaddingVertical(10).Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

    // ───────────────────────── Header ─────────────────────────

    private void ComposeHeader(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                var logo = ResolveFilePath(_model.Branding.LogoPath);
                if (logo is not null)
                {
                    row.ConstantItem(72).Height(40).AlignLeft().AlignMiddle()
                        .Image(logo).FitArea();
                    row.ConstantItem(14);
                }

                row.RelativeItem().AlignMiddle().Text(_model.Quote.Title)
                    .FontSize(20).Bold().FontColor(_primary);
            });

            col.Item().PaddingTop(5).LineHorizontal(2f).LineColor(_primary);

            col.Item().PaddingTop(6).Row(row =>
            {
                ComposeMetaSpan(row.RelativeItem(), "Taal", $"{_model.Quote.Language}");
                ComposeMetaSpan(row.RelativeItem().AlignCenter(), "Status", $"{_model.Quote.Status}");
                ComposeMetaSpan(row.RelativeItem().AlignRight(), "Datum",
                    $"{_model.Quote.CreatedAt:dd MMMM yyyy}");
            });
        });
    }

    private static void ComposeMetaSpan(IContainer container, string label, string value)
    {
        container.Text(text =>
        {
            text.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1));
            text.Span($"{label}: ").SemiBold();
            text.Span(value);
        });
    }

    // ───────────────────────── Footer ─────────────────────────

    private void ComposeFooter(IContainer container)
    {
        container.Column(col =>
        {
            col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
            col.Item().PaddingTop(4).Row(row =>
            {
                row.RelativeItem().Text(_model.Quote.Title)
                    .FontSize(7).FontColor(Colors.Grey.Lighten1);
                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("Pagina ").FontSize(7).FontColor(Colors.Grey.Lighten1);
                    text.CurrentPageNumber().FontSize(7).FontColor(Colors.Grey.Darken1).SemiBold();
                    text.Span(" / ").FontSize(7).FontColor(Colors.Grey.Lighten1);
                    text.TotalPages().FontSize(7).FontColor(Colors.Grey.Darken1).SemiBold();
                });
            });
        });
    }

    // ───────────────────────── Content ─────────────────────────

    private void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(14);

            if (_model.Days.Count == 0)
            {
                column.Item().PaddingVertical(40).AlignCenter()
                    .Text("Deze offerte bevat nog geen dagen.")
                    .Italic().FontColor(Colors.Grey.Darken1);
                return;
            }

            foreach (var block in _model.Days)
            {
                column.Item().EnsureSpace(80).Element(c => ComposeDayBlock(c, block));
            }

            // Prijsoverzicht
            column.Item().EnsureSpace(120).Element(ComposePriceOverview);
        });
    }

    // ───────────────────────── Dag blok ─────────────────────────

    private void ComposeDayBlock(IContainer container, DayBlock block)
    {
        container.Column(day =>
        {
            day.Spacing(6);

            // Dag header
            var title = string.IsNullOrWhiteSpace(block.Day.Title)
                ? $"Dag {block.Day.DayNumber}"
                : $"Dag {block.Day.DayNumber} \u2014 {block.Day.Title}";

            day.Item().Background(_primary).Padding(8).Row(row =>
            {
                row.RelativeItem().AlignMiddle().Text(title)
                    .FontSize(12).Bold().FontColor(Colors.White);
                row.ConstantItem(150).AlignRight().AlignMiddle()
                    .Text(block.Day.Date.ToString("dddd d MMMM yyyy"))
                    .FontSize(8).FontColor(Colors.White);
            });

            // Omschrijving
            if (!string.IsNullOrWhiteSpace(block.Day.Description))
            {
                day.Item().PaddingHorizontal(2).PaddingTop(2)
                    .Text(block.Day.Description!)
                    .FontSize(9).FontColor(Colors.Grey.Darken2).LineHeight(1.4f);
            }

            // Transport
            if (block.Transports.Count > 0)
            {
                day.Item().Element(c => ComposeTransportSection(c, block.Transports));
            }

            // Accommodaties
            foreach (var acc in block.Accommodations)
            {
                day.Item().Element(c => ComposeAccommodationCard(c, acc));
            }
        });
    }

    // ───────────────────────── Transport ─────────────────────────

    private void ComposeTransportSection(IContainer container, List<Logic.Models.Transport> transports)
    {
        container.Background(_lightBg).Padding(10).Column(col =>
        {
            col.Item().Text("Transport")
                .FontSize(10).SemiBold().FontColor(_accent);
            col.Item().PaddingTop(5);

            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(70);
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.ConstantColumn(70);
                });

                // Header
                ComposeTableHeader(table, "Type", "Vertrek", "Aankomst", "Details", "Prijs");

                table.Cell().ColumnSpan(5).PaddingBottom(2)
                    .LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                foreach (var t in transports)
                {
                    table.Cell().PaddingVertical(3).Text($"{t.Type}").FontSize(8.5f).SemiBold();
                    table.Cell().PaddingVertical(3).Text(t.DepartureLocation).FontSize(8.5f);
                    table.Cell().PaddingVertical(3).Text(t.ArrivalLocation).FontSize(8.5f);

                    var details = t.FlightNumber is not null
                        ? $"{t.FlightNumber} \u00b7 {t.Airline}"
                        : "\u2013";
                    table.Cell().PaddingVertical(3).Text(details).FontSize(8.5f)
                        .FontColor(Colors.Grey.Darken1);

                    var price = t.Price.HasValue ? $"\u20ac {t.Price.Value:0.00}" : "\u2013";
                    table.Cell().PaddingVertical(3).AlignRight().Text(price).FontSize(8.5f);
                }
            });
        });
    }

    // ───────────────────────── Accommodatie ─────────────────────────

    private void ComposeAccommodationCard(IContainer container, AccommodationBlock acc)
    {
        var imagePath = ResolveFilePath(acc.Accommodation.ImagePath);

        container.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Column(col =>
        {
            // Afbeelding + info naast elkaar als er een afbeelding is
            if (imagePath is not null)
            {
                col.Item().Row(row =>
                {
                    row.ConstantItem(180).Height(120).Image(imagePath).FitArea();
                    row.RelativeItem().Padding(10).Column(info =>
                    {
                        ComposeAccommodationInfo(info, acc);
                    });
                });
            }
            else
            {
                col.Item().Padding(10).Column(info =>
                {
                    ComposeAccommodationInfo(info, acc);
                });
            }

            // Kamertypes tabel altijd over de volle breedte
            if (acc.RoomTypes.Count > 0)
            {
                col.Item().PaddingHorizontal(10).PaddingBottom(10)
                    .Element(c => ComposeRoomTypeTable(c, acc.RoomTypes));
            }
        });
    }

    private void ComposeAccommodationInfo(ColumnDescriptor info, AccommodationBlock acc)
    {
        info.Spacing(2);

        info.Item().Text(acc.Accommodation.Name)
            .FontSize(11).SemiBold().FontColor(_accent);

        if (!string.IsNullOrWhiteSpace(acc.Accommodation.Address))
        {
            info.Item().Text(acc.Accommodation.Address!)
                .FontSize(8).FontColor(Colors.Grey.Darken1);
        }

        if (!string.IsNullOrWhiteSpace(acc.Accommodation.Description))
        {
            info.Item().PaddingTop(3).Text(acc.Accommodation.Description!)
                .FontSize(8.5f).FontColor(Colors.Grey.Darken2).LineHeight(1.3f);
        }
    }

    private void ComposeRoomTypeTable(IContainer container, List<Logic.Models.RoomType> roomTypes)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(c =>
            {
                c.RelativeColumn(3);
                c.RelativeColumn();
                c.RelativeColumn();
            });

            // Header
            table.Cell().Background(_primary).Padding(5)
                .Text("Kamertype").FontSize(7.5f).SemiBold().FontColor(Colors.White);
            table.Cell().Background(_primary).Padding(5)
                .Text("Prijs / nacht").FontSize(7.5f).SemiBold().FontColor(Colors.White);
            table.Cell().Background(_primary).Padding(5)
                .Text("Capaciteit").FontSize(7.5f).SemiBold().FontColor(Colors.White);

            foreach (var rt in roomTypes)
            {
                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                    .Padding(5).Text(rt.Name).FontSize(8.5f);
                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                    .Padding(5).Text($"\u20ac {rt.PricePerNight:0.00}").FontSize(8.5f);
                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                    .Padding(5).Text($"{rt.Capacity} pers.").FontSize(8.5f);
            }
        });
    }

    // ───────────────────────── Prijsoverzicht ─────────────────────────

    private void ComposePriceOverview(IContainer container)
    {
        var accEntries = _model.Days
            .SelectMany(d => d.Accommodations)
            .Where(a => a.RoomTypes.Count > 0)
            .ToList();

        var transportEntries = _model.Days
            .SelectMany(d => d.Transports)
            .Where(t => t.Price.HasValue)
            .ToList();

        if (accEntries.Count == 0 && transportEntries.Count == 0)
        {
            return;
        }

        container.Column(col =>
        {
            col.Spacing(10);

            // Sectie header
            col.Item().Background(_primary).Padding(8)
                .Text("Prijsoverzicht")
                .FontSize(13).Bold().FontColor(Colors.White);

            // ── Transport tabel ──
            if (transportEntries.Count > 0)
            {
                col.Item().PaddingTop(2).Text("Transport")
                    .FontSize(10).SemiBold().FontColor(_accent);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(70);
                        c.RelativeColumn();
                        c.RelativeColumn();
                        c.ConstantColumn(80);
                    });

                    table.Cell().Background(_lightBg).Padding(5)
                        .Text("Type").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                    table.Cell().Background(_lightBg).Padding(5)
                        .Text("Vertrek").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                    table.Cell().Background(_lightBg).Padding(5)
                        .Text("Aankomst").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                    table.Cell().Background(_lightBg).Padding(5).AlignRight()
                        .Text("Prijs").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);

                    foreach (var t in transportEntries)
                    {
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5)
                            .Text($"{t.Type}").FontSize(8.5f).SemiBold();
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5)
                            .Text(t.DepartureLocation).FontSize(8.5f);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5)
                            .Text(t.ArrivalLocation).FontSize(8.5f);
                        table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5)
                            .AlignRight()
                            .Text($"\u20ac {t.Price!.Value:0.00}").FontSize(8.5f);
                    }
                });

                // Subtotaal transport
                col.Item().PaddingTop(2).AlignRight()
                    .Text($"Subtotaal transport: \u20ac {_model.IndicativeTransportTotal:N2}")
                    .FontSize(9).SemiBold().FontColor(Colors.Grey.Darken2);
            }

            // ── Accommodatie tabel ──
            if (accEntries.Count > 0)
            {
                col.Item().PaddingTop(4).Text("Accommodaties")
                    .FontSize(10).SemiBold().FontColor(_accent);

                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(3);
                        c.RelativeColumn(3);
                        c.RelativeColumn();
                        c.ConstantColumn(80);
                    });

                    table.Cell().Background(_lightBg).Padding(5)
                        .Text("Accommodatie").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                    table.Cell().Background(_lightBg).Padding(5)
                        .Text("Kamertype").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                    table.Cell().Background(_lightBg).Padding(5)
                        .Text("Capaciteit").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);
                    table.Cell().Background(_lightBg).Padding(5).AlignRight()
                        .Text("Prijs / nacht").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken2);

                    foreach (var entry in accEntries)
                    {
                        var isFirst = true;
                        foreach (var rt in entry.RoomTypes.OrderBy(r => r.PricePerNight))
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                .Text(isFirst ? entry.Accommodation.Name : "")
                                .FontSize(8.5f).SemiBold();
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                .Text(rt.Name).FontSize(8.5f);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                .Text($"{rt.Capacity} pers.").FontSize(8.5f);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5)
                                .AlignRight()
                                .Text($"\u20ac {rt.PricePerNight:0.00}").FontSize(8.5f);

                            isFirst = false;
                        }
                    }
                });

                // Subtotaal accommodaties
                var accFromTotal = accEntries.Sum(e => e.RoomTypes.Min(r => r.PricePerNight));
                col.Item().PaddingTop(2).AlignRight()
                    .Text($"Subtotaal accommodaties (goedkoopste opties): \u20ac {accFromTotal:N2} / nacht")
                    .FontSize(9).SemiBold().FontColor(Colors.Grey.Darken2);
            }

            // ── Totaalbalk ──
            col.Item().PaddingTop(4).Background(_primary).Padding(10).Row(row =>
            {
                row.RelativeItem().AlignLeft().AlignMiddle()
                    .Text("Indicatie totaal")
                    .FontSize(10).SemiBold().FontColor(Colors.White);

                var grandTotal = _model.IndicativeTransportTotal + _model.IndicativeAccommodationTotal;
                row.ConstantItem(180).AlignRight().AlignMiddle()
                    .Text($"\u20ac {grandTotal:N2}")
                    .FontSize(13).Bold().FontColor(Colors.White);
            });

            // Toelichting
            col.Item().PaddingTop(4)
                .Text("* Prijzen zijn indicatief en onder voorbehoud van beschikbaarheid. " +
                      "Accommodatietotaal betreft de goedkoopste kameroptie per accommodatie per nacht.")
                .FontSize(7).FontColor(Colors.Grey.Darken1).Italic();
        });
    }

    // ───────────────────────── Helpers ─────────────────────────

    private static void ComposeTableHeader(TableDescriptor table, params string[] headers)
    {
        foreach (var h in headers)
        {
            table.Cell().PaddingBottom(4)
                .Text(h).FontSize(7.5f).SemiBold().FontColor(Colors.Grey.Darken1);
        }
    }

    /// <summary>Fysiek pad naar een bestand in wwwroot, of null.</summary>
    private string? ResolveFilePath(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return null;
        }

        if (relativePath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var relative = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var full = Path.Combine(_webRootPath, relative);
        return File.Exists(full) ? full : null;
    }
}
