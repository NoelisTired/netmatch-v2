using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Presentation.Infrastructure;
using QuestPDF.Infrastructure;

// QuestPDF Community-licentie (gratis voor dit gebruik) — FR-11 PDF-generatie.
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Geen ResourcesPath: de SDK noemt SharedResource.*.resx naar het type
// (Presentation.SharedResource.{cultuur}), dus de localizer zoekt daar
// zonder "Resources"-prefix.
builder.Services.AddLocalization();
builder.Services.AddControllersWithViews()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization();

// Ondersteunde talen: Nederlands (default), Engels, Duits, Frans.
var supportedCultures = new[] { "nl", "en", "de", "fr" };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.SetDefaultCulture("nl")
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
    // Cookie eerst, daarna Accept-Language van de browser.
    options.RequestCultureProviders =
    [
        new CookieRequestCultureProvider(),
        new AcceptLanguageHeaderRequestCultureProvider()
    ];
});

// Datalaag-keuze via DataProvider (Fake = in-memory, SqlServer = echte DB).
builder.Services.AddNetMatchDataAccess(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

var localizationOptions = app.Services
    .GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Quote}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();