using System.Data;
using Interface.DataInterfaces;
using Interface.DTO;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories;

/// <summary>
/// Datatoegang voor reisdagen via ruwe SQL (tabel <c>dag</c>). Alle queries
/// filteren op <c>deleted_at IS NULL</c>. snake_case kolomnaamgeving.
/// </summary>
public class DayRepository : IDayRepository
{
    private readonly string _connectionString;

    public DayRepository(string connectionString)
    {
        _connectionString = connectionString
            ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public IEnumerable<DayDto> GetByQuote(int quoteId)
    {
        var days = new List<DayDto>();
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"SELECT id, offerte_id, dagnummer, datum, titel, omschrijving, created_at, updated_at
                  FROM dag
                  WHERE deleted_at IS NULL AND offerte_id = @QuoteId
                  ORDER BY dagnummer",
                connection);
            command.Parameters.AddWithValue("@QuoteId", quoteId);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                days.Add(ReadDto(reader));
            }
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan reisdagen niet ophalen.", ex);
        }
        return days;
    }

    public DayDto? GetById(int id)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"SELECT id, offerte_id, dagnummer, datum, titel, omschrijving, created_at, updated_at
                  FROM dag
                  WHERE id = @Id AND deleted_at IS NULL",
                connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? ReadDto(reader) : null;
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan reisdag niet ophalen.", ex);
        }
    }

    public int Add(CreateDayDto dto)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"INSERT INTO dag (offerte_id, dagnummer, datum, titel, omschrijving, created_at, updated_at)
                  OUTPUT INSERTED.id
                  VALUES (@QuoteId, @DayNumber, @Date, @Title, @Description, SYSUTCDATETIME(), SYSUTCDATETIME())",
                connection);
            command.Parameters.AddWithValue("@QuoteId", dto.QuoteId);
            command.Parameters.AddWithValue("@DayNumber", dto.DayNumber);
            command.Parameters.AddWithValue("@Date", dto.Date);
            command.Parameters.AddWithValue("@Title", (object?)dto.Title ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object?)dto.Description ?? DBNull.Value);
            connection.Open();
            var result = command.ExecuteScalar();
            return result is null ? 0 : Convert.ToInt32(result);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan reisdag niet opslaan.", ex);
        }
    }

    public void Update(int id, CreateDayDto dto)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"UPDATE dag
                  SET dagnummer = @DayNumber, datum = @Date, titel = @Title,
                      omschrijving = @Description, updated_at = SYSUTCDATETIME()
                  WHERE id = @Id AND deleted_at IS NULL",
                connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@DayNumber", dto.DayNumber);
            command.Parameters.AddWithValue("@Date", dto.Date);
            command.Parameters.AddWithValue("@Title", (object?)dto.Title ?? DBNull.Value);
            command.Parameters.AddWithValue("@Description", (object?)dto.Description ?? DBNull.Value);
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan reisdag niet bijwerken.", ex);
        }
    }

    public void SoftDelete(int id)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"UPDATE dag
                  SET deleted_at = SYSUTCDATETIME()
                  WHERE id = @Id AND deleted_at IS NULL",
                connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan reisdag niet verwijderen.", ex);
        }
    }

    private static DayDto ReadDto(IDataReader reader)
    {
        return new DayDto
        {
            Id = reader.GetInt32(0),
            QuoteId = reader.GetInt32(1),
            DayNumber = reader.GetInt32(2),
            Date = reader.GetDateTime(3),
            Title = reader.IsDBNull(4) ? null : reader.GetString(4),
            Description = reader.IsDBNull(5) ? null : reader.GetString(5),
            CreatedAt = reader.GetDateTime(6),
            UpdatedAt = reader.GetDateTime(7)
        };
    }
}
