using Interface.DataInterfaces;
using Interface.DTO;
using Logic.Enums;
using Logic.Models;

namespace Logic;

/// <summary>
/// Bedrijfslogica rond transportmiddelen (FR-03). Vertaalt tussen DTO's en
/// het <see cref="Transport"/> domeinmodel en bewaakt de validatieregels.
/// Geeft nette foutmeldingen terug i.p.v. excepties.
/// </summary>
public class TransportService
{
    private readonly ITransportRepository _transportRepository;

    public TransportService(ITransportRepository transportRepository)
    {
        _transportRepository = transportRepository;
    }

    public IEnumerable<Transport> GetTransportsForDay(int dayId)
    {
        return _transportRepository.GetByDay(dayId).Select(MapToModel);
    }

    public Transport? GetTransportById(int id)
    {
        var dto = _transportRepository.GetById(id);
        return dto is null ? null : MapToModel(dto);
    }

    /// <summary>
    /// Voegt transport toe. FR-03: geldig type (TC-05), vertrek/aankomst
    /// verplicht (TC-07), bij vlucht ook vluchtnummer+maatschappij (TC-06).
    /// </summary>
    public (string? Error, int Id) AddTransport(int dayId, string? type,
        string? departureLocation, string? arrivalLocation, string? flightNumber, string? airline,
        decimal? price = null)
    {
        if (!TransportEnumMapping.TryParse(type, out var parsedType))
        {
            return ("Kies een geldig transporttype: Vlucht, Bus, Trein of Eigen vervoer.", 0);
        }

        try
        {
            var transport = new Transport(dayId, parsedType, departureLocation,
                arrivalLocation, flightNumber, airline, price);

            var id = _transportRepository.Add(new CreateTransportDto
            {
                DayId = transport.DayId,
                Type = transport.Type.ToCode(),
                DepartureLocation = transport.DepartureLocation,
                ArrivalLocation = transport.ArrivalLocation,
                FlightNumber = transport.FlightNumber,
                Airline = transport.Airline,
                Price = transport.Price
            });

            return (null, id);
        }
        catch (ArgumentException ex)
        {
            return (ex.Message, 0);
        }
    }

    public string? UpdateTransport(int id, string? type, string? departureLocation,
        string? arrivalLocation, string? flightNumber, string? airline, decimal? price = null)
    {
        var existing = _transportRepository.GetById(id);
        if (existing is null)
        {
            return "Transport niet gevonden.";
        }

        if (!TransportEnumMapping.TryParse(type, out var parsedType))
        {
            return "Kies een geldig transporttype: Vlucht, Bus, Trein of Eigen vervoer.";
        }

        try
        {
            var transport = new Transport(existing.DayId, parsedType, departureLocation,
                arrivalLocation, flightNumber, airline, price);

            _transportRepository.Update(id, new CreateTransportDto
            {
                DayId = transport.DayId,
                Type = transport.Type.ToCode(),
                DepartureLocation = transport.DepartureLocation,
                ArrivalLocation = transport.ArrivalLocation,
                FlightNumber = transport.FlightNumber,
                Airline = transport.Airline,
                Price = transport.Price
            });

            return null;
        }
        catch (ArgumentException ex)
        {
            return ex.Message;
        }
    }

    public void DeleteTransport(int id)
    {
        _transportRepository.SoftDelete(id);
    }

    private static Transport MapToModel(TransportDto dto)
    {
        TransportEnumMapping.TryParse(dto.Type, out var type);
        return new Transport(dto.Id, dto.DayId, type, dto.DepartureLocation,
            dto.ArrivalLocation, dto.FlightNumber, dto.Airline, dto.Price,
            dto.CreatedAt, dto.UpdatedAt);
    }
}
