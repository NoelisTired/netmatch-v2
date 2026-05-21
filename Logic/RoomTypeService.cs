using Interface.DataInterfaces;
using Interface.DTO;
using Logic.Models;

namespace Logic;

/// <summary>
/// Bedrijfslogica rond kamertypes (FR-04). Vertaalt tussen DTO's en het
/// <see cref="RoomType"/> domeinmodel en bewaakt de validatieregels.
/// </summary>
public class RoomTypeService
{
    private readonly IRoomTypeRepository _roomTypeRepository;

    public RoomTypeService(IRoomTypeRepository roomTypeRepository)
    {
        _roomTypeRepository = roomTypeRepository;
    }

    public IEnumerable<RoomType> GetRoomTypesForAccommodation(int accommodationId)
    {
        return _roomTypeRepository.GetByAccommodation(accommodationId).Select(MapToModel);
    }

    public RoomType? GetRoomTypeById(int id)
    {
        var dto = _roomTypeRepository.GetById(id);
        return dto is null ? null : MapToModel(dto);
    }

    /// <summary>
    /// Voegt een kamertype toe. FR-04: naam en prijs per nacht verplicht (TC-09).
    /// </summary>
    public (string? Error, int Id) AddRoomType(int accommodationId, string? name,
        decimal? pricePerNight, int capacity)
    {
        try
        {
            var roomType = new RoomType(accommodationId, name, pricePerNight, capacity);

            var id = _roomTypeRepository.Add(new CreateRoomTypeDto
            {
                AccommodationId = roomType.AccommodationId,
                Name = roomType.Name,
                PricePerNight = roomType.PricePerNight,
                Capacity = roomType.Capacity
            });

            return (null, id);
        }
        catch (ArgumentException ex)
        {
            return (ex.Message, 0);
        }
    }

    public string? UpdateRoomType(int id, string? name, decimal? pricePerNight, int capacity)
    {
        var existing = _roomTypeRepository.GetById(id);
        if (existing is null)
        {
            return "Kamertype niet gevonden.";
        }

        try
        {
            var roomType = new RoomType(existing.AccommodationId, name, pricePerNight, capacity);

            _roomTypeRepository.Update(id, new CreateRoomTypeDto
            {
                AccommodationId = roomType.AccommodationId,
                Name = roomType.Name,
                PricePerNight = roomType.PricePerNight,
                Capacity = roomType.Capacity
            });

            return null;
        }
        catch (ArgumentException ex)
        {
            return ex.Message;
        }
    }

    public void DeleteRoomType(int id)
    {
        _roomTypeRepository.SoftDelete(id);
    }

    private static RoomType MapToModel(RoomTypeDto dto)
    {
        return new RoomType(dto.Id, dto.AccommodationId, dto.Name, dto.PricePerNight,
            dto.Capacity, dto.CreatedAt, dto.UpdatedAt);
    }
}
