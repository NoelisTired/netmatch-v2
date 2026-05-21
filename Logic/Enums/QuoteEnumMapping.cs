namespace Logic.Enums;

/// <summary>
/// Vertaalt de offerte-enums van/naar hun opgeslagen stringwaarden en parst
/// invoer van de gebruiker. Houdt de mapping op één plek (geen magic strings
/// verspreid over de lagen).
/// </summary>
public static class QuoteEnumMapping
{
    /// <summary>Taalcode zoals opgeslagen in kolom <c>taal</c>.</summary>
    public static string ToCode(this LanguageType language) => language switch
    {
        LanguageType.Nederlands => "NL",
        LanguageType.Engels => "EN",
        LanguageType.Duits => "DE",
        LanguageType.Frans => "FR",
        _ => "NL"
    };

    /// <summary>
    /// Parst een taalcode (NL/EN/DE/FR) of weergavenaam (Nederlands/Engels/...).
    /// Hoofdletterongevoelig. Geeft <c>false</c> bij een niet-ondersteunde taal.
    /// </summary>
    public static bool TryParseLanguage(string? input, out LanguageType language)
    {
        language = LanguageType.Nederlands;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        switch (input.Trim().ToUpperInvariant())
        {
            case "NL":
            case "NEDERLANDS":
                language = LanguageType.Nederlands;
                return true;
            case "EN":
            case "ENGELS":
                language = LanguageType.Engels;
                return true;
            case "DE":
            case "DUITS":
                language = LanguageType.Duits;
                return true;
            case "FR":
            case "FRANS":
                language = LanguageType.Frans;
                return true;
            default:
                return false;
        }
    }

    /// <summary>Statuswaarde zoals opgeslagen in kolom <c>status</c>.</summary>
    public static string ToCode(this StatusType status) => status switch
    {
        StatusType.Concept => "concept",
        StatusType.Definitief => "definitief",
        _ => "concept"
    };

    /// <summary>Parst een statuswaarde; valt terug op <see cref="StatusType.Concept"/>.</summary>
    public static StatusType ParseStatus(string? input)
    {
        return string.Equals(input?.Trim(), "definitief", StringComparison.OrdinalIgnoreCase)
            ? StatusType.Definitief
            : StatusType.Concept;
    }
}
