using Logic.Models;

namespace Presentation.Models;

/// <summary>
/// Samengestelde weergave van een volledige offerte op één pagina:
/// offerte → dagen → per dag transport + accommodatie → per accommodatie
/// kamertypes. Voorkomt het doorklikken door losse overzichtspagina's.
/// </summary>
public class QuoteOverviewViewModel
{
    public Quote Quote { get; set; } = null!;

    /// <summary>Huisstijl van het reisbureau (FR-12), toegepast op de weergave.</summary>
    public Branding Branding { get; set; } = null!;

    public List<DayBlock> Days { get; set; } = new();

    /// <summary>Indicatie: som van de goedkoopste kamerprijs per accommodatie.</summary>
    public decimal IndicativeAccommodationTotal { get; set; }
}

public class DayBlock
{
    public Day Day { get; set; } = null!;
    public List<Transport> Transports { get; set; } = new();
    public List<AccommodationBlock> Accommodations { get; set; } = new();
}

public class AccommodationBlock
{
    public Accommodation Accommodation { get; set; } = null!;
    public List<RoomType> RoomTypes { get; set; } = new();
}
