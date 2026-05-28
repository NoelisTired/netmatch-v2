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
    private readonly Color _cardBg;

    public QuotePdfDocument(QuoteOverviewViewModel model, string webRootPath)
    {
        _model = model;
        _webRootPath = webRootPath;
        _primary = ParseColor(model.Branding.PrimaryColor, Colors.Blue.Medium);
        _accent = ParseColor(model.Branding.AccentColor, Colors.Grey.Darken1);
        _lightBg = Color.FromHex("#F8F9FA");
        _cardBg = Color.FromHex("#FAFBFC");
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
            page.Content().PaddingVertical(12).Element(ComposeContent);
            page.Footer().Element(ComposeFooter);
        });
    }

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
                row.RelativeItem().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1));
                    text.Span("Taal: ").SemiBold();
                    text.Span($"{_model.Quote.Language}");
                });

                row.RelativeItem().AlignCenter().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1));
                    text.Span("Status: ").SemiBold();
                    text.Span($"{_model.Quote.Status}");
                });

                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1));
                    text.Span("Datum: ").SemiBold();
                    text.Span($"{_model.Quote.CreatedAt:dd MMMM yyyy}");
                });
            });
        });
    }

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

    private void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(16);

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

            if (_model.IndicativeAccommodationTotal > 0)
            {
                column.Item().PaddingTop(4).Element(ComposeTotalBar);
            }
        });
    }

    private void ComposeDayBlock(IContainer container, DayBlock block)
    {
        container.Column(day =>
        {
            day.Spacing(8);

            // Dag header: gekleurde balk met dagnummer + titel
            var title = string.IsNullOrWhiteSpace(block.Day.Title)
                ? $"Dag {block.Day.DayNumber}"
                : $"Dag {block.Day.DayNumber} \u2014 {block.Day.Title}";

            day.Item().Background(_primary).Padding(8).Row(row =>
            {
                row.RelativeItem().AlignMiddle().Text(title)
                    .FontSize(12).Bold().FontColor(Colors.White);
                row.ConstantItem(140).AlignRight().AlignMiddle()
                    .Text(block.Day.Date.ToString("dddd d MMMM yyyy"))
                    .FontSize(8).FontColor(Colors.White);
            });

            // Dag omschrijving
            if (!string.IsNullOrWhiteSpace(block.Day.Description))
            {
                day.Item().PaddingHorizontal(4).PaddingTop(2)
                    .Text(block.Day.Description!)
                    .FontSize(9).FontColor(Colors.Grey.Darken2).LineHeight(1.4f);
            }

            // Transport sectie
            if (block.Transports.Count > 0)
            {
                day.Item().Element(c => ComposeTransportSection(c, block.Transports));
            }

            // Accommodatie secties
            foreach (var acc in block.Accommodations)
            {
                day.Item().Element(c => ComposeAccommodationSection(c, acc));
            }
        });
    }

    private void ComposeTransportSection(IContainer container, List<Logic.Models.Transport> transports)
    {
        container.Background(_lightBg).Padding(10).Column(col =>
        {
            col.Item().Text("\u2708  Transport")
                .FontSize(10).SemiBold().FontColor(_accent);
            col.Item().PaddingTop(6);

            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(80);
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.RelativeColumn();
                });

                // Header rij
                table.Cell().PaddingBottom(4).Text("Type")
                    .FontSize(7.5f).SemiBold().FontColor(Colors.Grey.Darken1);
                table.Cell().PaddingBottom(4).Text("Vertrek")
                    .FontSize(7.5f).SemiBold().FontColor(Colors.Grey.Darken1);
                table.Cell().PaddingBottom(4).Text("Aankomst")
                    .FontSize(7.5f).SemiBold().FontColor(Colors.Grey.Darken1);
                table.Cell().PaddingBottom(4).Text("Details")
                    .FontSize(7.5f).SemiBold().FontColor(Colors.Grey.Darken1);

                // Lijn onder header
                table.Cell().ColumnSpan(4).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

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
                }
            });
        });
    }

    private void ComposeAccommodationSection(IContainer container, AccommodationBlock acc)
    {
        container.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Background(_cardBg).Column(col =>
        {
            // Afbeelding bovenaan de kaart
            var imagePath = ResolveFilePath(acc.Accommodation.ImagePath);
            if (imagePath is not null)
            {
                col.Item().Height(140).Image(imagePath).FitArea();
            }

            col.Item().Padding(10).Column(content =>
            {
                content.Spacing(3);

                // Naam
                content.Item().Text($"\ud83c\udfe8  {acc.Accommodation.Name}")
                    .FontSize(11).SemiBold().FontColor(_accent);

                // Adres
                if (!string.IsNullOrWhiteSpace(acc.Accommodation.Address))
                {
                    content.Item().Text(acc.Accommodation.Address!)
                        .FontSize(8).FontColor(Colors.Grey.Darken1);
                }

                // Omschrijving
                if (!string.IsNullOrWhiteSpace(acc.Accommodation.Description))
                {
                    content.Item().PaddingTop(3).Text(acc.Accommodation.Description!)
                        .FontSize(8.5f).FontColor(Colors.Grey.Darken2).LineHeight(1.35f);
                }

                // Kamertypes tabel
                if (acc.RoomTypes.Count > 0)
                {
                    content.Item().PaddingTop(8).Table(table =>
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

                        foreach (var rt in acc.RoomTypes)
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
            });
        });
    }

    private void ComposeTotalBar(IContainer container)
    {
        container.Background(_primary).Padding(12).Row(row =>
        {
            row.RelativeItem().AlignLeft().AlignMiddle()
                .Text("Indicatie accommodatiekosten")
                .FontSize(11).SemiBold().FontColor(Colors.White);
            row.RelativeItem().AlignRight().AlignMiddle()
                .Text($"\u20ac {_model.IndicativeAccommodationTotal:N2} per nacht")
                .FontSize(13).Bold().FontColor(Colors.White);
        });
    }

    /// <summary>Fysiek pad naar een bestand in wwwroot, of null.</summary>
    private string? ResolveFilePath(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return null;
        }

        // QuestPDF ondersteunt alleen rasterformaten
        if (relativePath.EndsWith(".svg", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var relative = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var full = Path.Combine(_webRootPath, relative);
        return File.Exists(full) ? full : null;
    }
}
