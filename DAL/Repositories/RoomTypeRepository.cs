using System.Data;
using Interface.DataInterfaces;
using Interface.DTO;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories;

/// <summary>
/// Datatoegang voor kamertypes via ruwe SQL (tabel <c>kamertype</c>).
/// Alle queries filteren op <c>deleted_at IS NULL</c>. snake_case kolommen.
/// </summary>
public class RoomTypeRepository : IRoomTypeRepository
{
    private readonly string _connectionString;

    public RoomTypeRepository(string connectionString)
    {
        _connectionString = connectionString
            ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public IEnumerable<RoomTypeDto> GetByAccommodation(int accommodationId)
    {
        var items = new List<RoomTypeDto>();
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"SELECT id, accommodatie_id, naam, prijs_per_nacht, capaciteit, created_at, updated_at
                  FROM kamertype
                  WHERE deleted_at IS NULL AND accommodatie_id = @AccommodationId
                  ORDER BY id",
                connection);
            command.Parameters.AddWithValue("@AccommodationId", accommodationId);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(ReadDto(reader));
            }
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan kamertypes niet ophalen.", ex);
        }
        return items;
    }

    public RoomTypeDto? GetById(int id)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"SELECT id, accommodatie_id, naam, prijs_per_nacht, capaciteit, created_at, updated_at
                  FROM kamertype
                  WHERE id = @Id AND deleted_at IS NULL",
                connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? ReadDto(reader) : null;
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan kamertype niet ophalen.", ex);
        }
    }

    public int Add(CreateRoomTypeDto dto)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"INSERT INTO kamertype (accommodatie_id, naam, prijs_per_nacht, capaciteit, created_at, updated_at)
                  OUTPUT INSERTED.id
                  VALUES (@AccommodationId, @Name, @PricePerNight, @Capacity, SYSUTCDATETIME(), SYSUTCDATETIME())",
                connection);
            command.Parameters.AddWithValue("@AccommodationId", dto.AccommodationId);
            command.Parameters.AddWithValue("@Name", dto.Name);
            command.Parameters.AddWithValue("@PricePerNight", dto.PricePerNight);
            command.Parameters.AddWithValue("@Capacity", dto.Capacity);
            connection.Open();
            var result = command.ExecuteScalar();
            return result is null ? 0 : Convert.ToInt32(result);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan kamertype niet opslaan.", ex);
        }
    }

    public void Update(int id, CreateRoomTypeDto dto)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"UPDATE kamertype
                  SET naam = @Name, prijs_per_nacht = @PricePerNight, capaciteit = @Capacity,
                      updated_at = SYSUTCDATETIME()
                  WHERE id = @Id AND deleted_at IS NULL",
                connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", dto.Name);
            command.Parameters.AddWithValue("@PricePerNight", dto.PricePerNight);
            command.Parameters.AddWithValue("@Capacity", dto.Capacity);
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan kamertype niet bijwerken.", ex);
        }
    }

    public void SoftDelete(int id)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"UPDATE kamertype
                  SET deleted_at = SYSUTCDATETIME()
                  WHERE id = @Id AND deleted_at IS NULL",
                connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan kamertype niet verwijderen.", ex);
        }
    }

    private static RoomTypeDto ReadDto(IDataReader reader)
    {
        return new RoomTypeDto
        {
            Id = reader.GetInt32(0),
            AccommodationId = reader.GetInt32(1),
            Name = reader.GetString(2),
            PricePerNight = reader.GetDecimal(3),
            Capacity = reader.GetInt32(4),
            CreatedAt = reader.GetDateTime(5),
            UpdatedAt = reader.GetDateTime(6)
        };
    }
}
