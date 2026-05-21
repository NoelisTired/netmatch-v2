using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

/// <summary>
/// Zet de taalkeuze van de gebruiker als cookie (FR-01-talen NL/EN/DE/FR
/// voor de hele UI) en stuurt terug naar de vorige pagina.
/// </summary>
public class CultureController : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Set(string culture, string? returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true
            });

        return LocalRedirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
    }
}
