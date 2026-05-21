namespace Logic.Enums;

/// <summary>
/// Toegestane transporttypes (FR-03). Transport = langere verplaatsing;
/// lokale ritten vallen onder transfer (FR-05).
/// </summary>
public enum TransportType
{
    Vlucht,
    Bus,
    Trein,
    EigenVervoer
}
