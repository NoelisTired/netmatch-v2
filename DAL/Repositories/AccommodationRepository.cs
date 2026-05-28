using System.Data;
using Interface.DataInterfaces;
using Interface.DTO;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories;

/// <summary>
/// Datatoegang voor accommodaties via ruwe SQL (tabel <c>accommodatie</c>).
/// Alle queries filteren op <c>deleted_at IS NULL</c>. snake_case kolommen.
/// </summary>
public class AccommodationRepository : IAccommodationRepository
{
    private readonly string _connectionString;

    public AccommodationRepository(string connectionString)
    {
        _connectionString = connectionString
            ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public IEnumerable<AccommodationDto> GetByDay(int dayId)
    {
        var items = new List<AccommodationDto>();
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"SELECT id, dag_id, naam, adres, omschrijving, afbeelding_pad, created_at, updated_at
                  FROM accommodatie
                  WHERE deleted_at IS NULL AND dag_id = @DayId
                  ORDER BY id",
                connection);
            command.Parameters.AddWithValue("@DayId", dayId);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                items.Add(ReadDto(reader));
            }
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan accommodaties niet ophalen.", ex);
        }
        return items;
    }

    public AccommodationDto? GetById(int id)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"SELECT id, dag_id, naam, adres, omschrijving, afbeelding_pad, created_at, updated_at
                  FROM accommodatie
                  WHERE id = @Id AND deleted_at IS NULL",
                connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? ReadDto(reader) : null;
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan accommodatie niet ophalen.", ex);
        }
    }

    public int Add(CreateAccommodationDto dto)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"INSERT INTO accommodatie (dag_id, naam, adres, omschrijving, afbeelding_pad, created_at, updated_at)
                  OUTPUT INSERTED.id
                  VALUES (@DayId, @Name, @Address, @Description, @ImagePath, SYSUTCDATETIME(), SYSUTCDATETIME())",
                connection);
            command.Parameters.AddWithValue("@DayId", dto.DayId);
            command.Parameters.AddWithValue("@Name", dto.Name);
            command.Parameters.AddWithValue("@Address", (object?)dto.Address ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object?)dto.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@ImagePath", (object?)dto.ImagePath ?? DBNull.Value);
            connection.Open();
            var result = command.ExecuteScalar();
            return result is null ? 0 : Convert.ToInt32(result);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan accommodatie niet opslaan.", ex);
        }
    }

    public void Update(int id, CreateAccommodationDto dto)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"UPDATE accommodatie
                  SET naam = @Name, adres = @Address, omschrijving = @Description,
                      afbeelding_pad = @ImagePath, updated_at = SYSUTCDATETIME()
                  WHERE id = @Id AND deleted_at IS NULL",
                connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Name", dto.Name);
            command.Parameters.AddWithValue("@Address", (object?)dto.Address ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object?)dto.Description ?? DBNull.Value);
            command.Parameters.AddWithValue("@ImagePath", (object?)dto.ImagePath ?? DBNull.Value);
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan accommodatie niet bijwerken.", ex);
        }
    }

    public void SoftDelete(int id)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"UPDATE accommodatie
                  SET deleted_at = SYSUTCDATETIME()
                  WHERE id = @Id AND deleted_at IS NULL",
                connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan accommodatie niet verwijderen.", ex);
        }
    }

    private static AccommodationDto ReadDto(IDataReader reader)
    {
        return new AccommodationDto
        {
            Id = reader.GetInt32(0),
            DayId = reader.GetInt32(1),
            Name = reader.GetString(2),
            Address = reader.IsDBNull(3) ? null : reader.GetString(3),
            Description = reader.IsDBNull(4) ? null : reader.GetString(4),
            ImagePath = reader.IsDBNull(5) ? null : reader.GetString(5),
            CreatedAt = reader.GetDateTime(6),
            UpdatedAt = reader.GetDateTime(7)
        };
    }
}
