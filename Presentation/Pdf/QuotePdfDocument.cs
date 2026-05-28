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
        _lightBg = Color.FromHex("#F8F9FA");
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
            page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken3));

            page.Header().Element(ComposeHeader);
            page.Content().PaddingVertical(16).Element(ComposeContent);
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
                    row.ConstantItem(80).Height(44).AlignLeft().AlignMiddle()
                        .Image(logo).FitArea();
                    row.ConstantItem(16);
                }

                row.RelativeItem().AlignMiddle().Column(title =>
                {
                    title.Item().Text(_model.Quote.Title)
                        .FontSize(22).Bold().FontColor(_primary);
                });
            });

            col.Item().PaddingTop(6).LineHorizontal(1.5f).LineColor(_primary);

            col.Item().PaddingTop(8).DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.Grey.Darken1))
                .Row(row =>
            {
                row.RelativeItem().Text(text =>
                {
                    text.Span("Taal: ").SemiBold();
                    text.Span($"{_model.Quote.Language}");
                });

                row.RelativeItem().AlignCenter().Text(text =>
                {
                    text.Span("Status: ").SemiBold();
                    text.Span($"{_model.Quote.Status}");
                });

                row.RelativeItem().AlignRight().Text(text =>
                {
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
            col.Item().PaddingTop(6).Row(row =>
            {
                row.RelativeItem().Text(_model.Quote.Title)
                    .FontSize(8).FontColor(Colors.Grey.Lighten1);
                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.Span("Pagina ").FontSize(8).FontColor(Colors.Grey.Lighten1);
                    text.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Darken1).SemiBold();
                    text.Span(" / ").FontSize(8).FontColor(Colors.Grey.Lighten1);
                    text.TotalPages().FontSize(8).FontColor(Colors.Grey.Darken1).SemiBold();
                });
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(20);

            if (_model.Days.Count == 0)
            {
                column.Item().PaddingVertical(40).AlignCenter()
                    .Text("Deze offerte bevat nog geen dagen.")
                    .Italic().FontColor(Colors.Grey.Darken1);
                return;
            }

            foreach (var block in _model.Days)
            {
                column.Item().EnsureSpace(100).Element(c => ComposeDayBlock(c, block));
            }

            if (_model.IndicativeAccommodationTotal > 0)
            {
                column.Item().Element(ComposeTotalBar);
            }
        });
    }

    private void ComposeDayBlock(IContainer container, DayBlock block)
    {
        container.Column(day =>
        {
            // Dag header
            var title = string.IsNullOrWhiteSpace(block.Day.Title)
                ? $"Dag {block.Day.DayNumber}"
                : $"Dag {block.Day.DayNumber} — {block.Day.Title}";

            day.Item().Row(row =>
            {
                row.ConstantItem(4).Background(_primary).ExtendVertical();
                row.ConstantItem(8);
                row.RelativeItem().PaddingVertical(4).Column(header =>
                {
                    header.Item().Text(title).FontSize(14).Bold().FontColor(_primary);
                    header.Item().Text(block.Day.Date.ToString("dddd d MMMM yyyy"))
                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                });
            });

            // Dag omschrijving
            if (!string.IsNullOrWhiteSpace(block.Day.Description))
            {
                day.Item().PaddingTop(6).PaddingLeft(12)
                    .Text(block.Day.Description!).FontSize(10).FontColor(Colors.Grey.Darken2);
            }

            // Transport sectie
            if (block.Transports.Count > 0)
            {
                day.Item().PaddingTop(10).Element(c => ComposeTransportSection(c, block.Transports));
            }

            // Accommodatie secties
            foreach (var acc in block.Accommodations)
            {
                day.Item().PaddingTop(10).Element(c => ComposeAccommodationSection(c, acc));
            }
        });
    }

    private void ComposeTransportSection(IContainer container, List<Logic.Models.Transport> transports)
    {
        container.Background(_lightBg).Border(0.5f).BorderColor(Colors.Grey.Lighten2)
            .Padding(10).Column(col =>
        {
            col.Item().Text("Transport").FontSize(11).SemiBold().FontColor(_accent);
            col.Item().PaddingTop(4);

            col.Item().Table(table =>
            {
                table.ColumnsDefinition(c =>
                {
                    c.ConstantColumn(70);
                    c.RelativeColumn();
                    c.RelativeColumn();
                    c.RelativeColumn();
                });

                // Header rij
                table.Cell().Text("Type").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken1);
                table.Cell().Text("Vertrek").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken1);
                table.Cell().Text("Aankomst").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken1);
                table.Cell().Text("Details").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken1);

                foreach (var t in transports)
                {
                    // Separator
                    table.Cell().ColumnSpan(4).PaddingVertical(3)
                        .LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                    table.Cell().PaddingVertical(2).Text($"{t.Type}").FontSize(9);
                    table.Cell().PaddingVertical(2).Text(t.DepartureLocation).FontSize(9);
                    table.Cell().PaddingVertical(2).Text(t.ArrivalLocation).FontSize(9);

                    var details = t.FlightNumber is not null
                        ? $"{t.FlightNumber} · {t.Airline}"
                        : "–";
                    table.Cell().PaddingVertical(2).Text(details).FontSize(9)
                        .FontColor(Colors.Grey.Darken1);
                }
            });
        });
    }

    private void ComposeAccommodationSection(IContainer container, AccommodationBlock acc)
    {
        container.Border(0.5f).BorderColor(Colors.Grey.Lighten2).Column(col =>
        {
            // Accommodation header met optioneel afbeelding
            var imagePath = ResolveFilePath(acc.Accommodation.ImagePath);
            if (imagePath is not null)
            {
                col.Item().MaxHeight(160).Image(imagePath).FitArea();
            }

            col.Item().Padding(10).Column(content =>
            {
                content.Item().Text($"Accommodatie: {acc.Accommodation.Name}")
                    .FontSize(11).SemiBold().FontColor(_accent);

                if (!string.IsNullOrWhiteSpace(acc.Accommodation.Address))
                {
                    content.Item().PaddingTop(2).Text(acc.Accommodation.Address!)
                        .FontSize(9).FontColor(Colors.Grey.Darken1);
                }

                if (!string.IsNullOrWhiteSpace(acc.Accommodation.Description))
                {
                    content.Item().PaddingTop(4).Text(acc.Accommodation.Description!)
                        .FontSize(9).FontColor(Colors.Grey.Darken2);
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
                        table.Cell().Background(_lightBg).Padding(4)
                            .Text("Kamertype").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken1);
                        table.Cell().Background(_lightBg).Padding(4)
                            .Text("Prijs / nacht").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken1);
                        table.Cell().Background(_lightBg).Padding(4)
                            .Text("Capaciteit").FontSize(8).SemiBold().FontColor(Colors.Grey.Darken1);

                        foreach (var rt in acc.RoomTypes)
                        {
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                .Padding(4).Text(rt.Name).FontSize(9);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                .Padding(4).Text($"\u20ac {rt.PricePerNight:0.00}").FontSize(9);
                            table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2)
                                .Padding(4).Text($"{rt.Capacity} pers.").FontSize(9);
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
