using Interface.DataInterfaces;
using Interface.DTO;
using Logic.Models;

namespace Logic;

/// <summary>
/// Bedrijfslogica rond accommodaties (FR-04). Vertaalt tussen DTO's en het
/// <see cref="Accommodation"/> domeinmodel en bewaakt de validatieregels.
/// </summary>
public class AccommodationService
{
    private readonly IAccommodationRepository _accommodationRepository;

    public AccommodationService(IAccommodationRepository accommodationRepository)
    {
        _accommodationRepository = accommodationRepository;
    }

    public IEnumerable<Accommodation> GetAccommodationsForDay(int dayId)
    {
        return _accommodationRepository.GetByDay(dayId).Select(MapToModel);
    }

    public Accommodation? GetAccommodationById(int id)
    {
        var dto = _accommodationRepository.GetById(id);
        return dto is null ? null : MapToModel(dto);
    }

    /// <summary>Voegt een accommodatie toe. FR-04: naam verplicht (TC-08).</summary>
    public (string? Error, int Id) AddAccommodation(int dayId, string? name,
        string? address, string? description)
    {
        try
        {
            var accommodation = new Accommodation(dayId, name, address, description);

            var id = _accommodationRepository.Add(new CreateAccommodationDto
            {
                DayId = accommodation.DayId,
                Name = accommodation.Name,
                Address = accommodation.Address,
                Description = accommodation.Description
            });

            return (null, id);
        }
        catch (ArgumentException ex)
        {
            return (ex.Message, 0);
        }
    }

    public string? UpdateAccommodation(int id, string? name, string? address, string? description)
    {
        var existing = _accommodationRepository.GetById(id);
        if (existing is null)
        {
            return "Accommodatie niet gevonden.";
        }

        try
        {
            var accommodation = new Accommodation(existing.DayId, name, address, description);

            _accommodationRepository.Update(id, new CreateAccommodationDto
            {
                DayId = accommodation.DayId,
                Name = accommodation.Name,
                Address = accommodation.Address,
                Description = accommodation.Description
            });

            return null;
        }
        catch (ArgumentException ex)
        {
            return ex.Message;
        }
    }

    public void DeleteAccommodation(int id)
    {
        _accommodationRepository.SoftDelete(id);
    }

    private static Accommodation MapToModel(AccommodationDto dto)
    {
        return new Accommodation(dto.Id, dto.DayId, dto.Name, dto.Address, dto.Description,
            dto.CreatedAt, dto.UpdatedAt);
    }
}
