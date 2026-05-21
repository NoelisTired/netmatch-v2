using DAL.Memory;
using DAL.Repositories;
using Interface.DataInterfaces;
using Logic;

namespace Presentation.Infrastructure;

/// <summary>
/// Kiest de datalaag op basis van <c>DataProvider</c> (appsettings/env):
/// <c>Fake</c> = in-memory (app draait zonder DB), <c>SqlServer</c> = echte
/// database. Een onbekende waarde faalt hard i.p.v. stil. Patroon overgenomen
/// van MediDisplay.
/// </summary>
public static class DataAccessRegistration
{
    public static IServiceCollection AddNetMatchDataAccess(
        this IServiceCollection services, IConfiguration config)
    {
        var provider = config["DataProvider"] ?? "Fake";

        services.AddScoped<QuoteService>();
        services.AddScoped<DayService>();
        services.AddScoped<TransportService>();
        services.AddScoped<AccommodationService>();
        services.AddScoped<RoomTypeService>();
        services.AddScoped<BrandingService>();

        if (IsFakeProvider(provider))
        {
            // Singleton: in-memory data leeft zo lang de app draait.
            services.AddSingleton<IQuoteRepository, FakeQuoteRepository>();
            services.AddSingleton<IDayRepository, FakeDayRepository>();
            services.AddSingleton<ITransportRepository, FakeTransportRepository>();
            services.AddSingleton<IAccommodationRepository, FakeAccommodationRepository>();
            services.AddSingleton<IRoomTypeRepository, FakeRoomTypeRepository>();
            services.AddSingleton<IBrandingRepository, FakeBrandingRepository>();
            return services;
        }

        if (string.Equals(provider, "SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            var connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' ontbreekt in appsettings.");
            services.AddScoped<IQuoteRepository>(_ => new QuoteRepository(connectionString));
            services.AddScoped<IDayRepository>(_ => new DayRepository(connectionString));
            services.AddScoped<ITransportRepository>(_ => new TransportRepository(connectionString));
            services.AddScoped<IAccommodationRepository>(_ => new AccommodationRepository(connectionString));
            services.AddScoped<IRoomTypeRepository>(_ => new RoomTypeRepository(connectionString));
            services.AddScoped<IBrandingRepository>(_ => new BrandingRepository(connectionString));
            return services;
        }

        throw new InvalidOperationException(
            $"Onbekende DataProvider '{provider}'. Gebruik Fake of SqlServer.");
    }

    public static bool IsFakeProvider(string? provider) =>
        string.Equals(provider ?? "Fake", "Fake", StringComparison.OrdinalIgnoreCase);
}
