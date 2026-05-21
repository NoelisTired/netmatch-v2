namespace Logic.Enums;

/// <summary>
/// Vertaalt <see cref="TransportType"/> van/naar de opgeslagen stringwaarde
/// en parst gebruikersinvoer. Mapping op één plek (geen magic strings).
/// </summary>
public static class TransportEnumMapping
{
    /// <summary>Waarde zoals opgeslagen in kolom <c>type</c>.</summary>
    public static string ToCode(this TransportType type) => type switch
    {
        TransportType.Vlucht => "Vlucht",
        TransportType.Bus => "Bus",
        TransportType.Trein => "Trein",
        TransportType.EigenVervoer => "Eigen vervoer",
        _ => "Bus"
    };

    /// <summary>
    /// Parst een transporttype (label of code), hoofdletterongevoelig.
    /// Geeft <c>false</c> bij een niet-ondersteund type (FR-03 / TC-05).
    /// </summary>
    public static bool TryParse(string? input, out TransportType type)
    {
        type = TransportType.Bus;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        switch (input.Trim().ToUpperInvariant())
        {
            case "VLUCHT":
                type = TransportType.Vlucht;
                return true;
            case "BUS":
                type = TransportType.Bus;
                return true;
            case "TREIN":
                type = TransportType.Trein;
                return true;
            case "EIGEN VERVOER":
            case "EIGENVERVOER":
                type = TransportType.EigenVervoer;
                return true;
            default:
                return false;
        }
    }
}
