using System.Data;
using Interface.DataInterfaces;
using Interface.DTO;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories;

/// <summary>
/// Datatoegang voor offertes via ruwe SQL (geen ORM). Alle queries filteren
/// op <c>deleted_at IS NULL</c> zodat soft-deleted offertes nooit terugkomen.
/// Kolomnaamgeving is snake_case conform het databaseontwerp.
/// </summary>
public class QuoteRepository : IQuoteRepository
{
    private readonly string _connectionString;

    public QuoteRepository(string connectionString)
    {
        _connectionString = connectionString
            ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public IEnumerable<QuoteDto> GetAll()
    {
        var quotes = new List<QuoteDto>();
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"SELECT id, travelagent_id, titel, taal, status, created_at, updated_at
                  FROM offerte
                  WHERE deleted_at IS NULL
                  ORDER BY id DESC",
                connection);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                quotes.Add(ReadDto(reader));
            }
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan offertes niet ophalen.", ex);
        }
        return quotes;
    }

    public IEnumerable<QuoteDto> GetAllByTravelAgent(int travelAgentId)
    {
        var quotes = new List<QuoteDto>();
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"SELECT id, travelagent_id, titel, taal, status, created_at, updated_at
                  FROM offerte
                  WHERE deleted_at IS NULL AND travelagent_id = @TravelAgentId
                  ORDER BY id DESC",
                connection);
            command.Parameters.AddWithValue("@TravelAgentId", travelAgentId);
            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                quotes.Add(ReadDto(reader));
            }
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan offertes niet ophalen.", ex);
        }
        return quotes;
    }

    public QuoteDto? GetById(int id)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"SELECT id, travelagent_id, titel, taal, status, created_at, updated_at
                  FROM offerte
                  WHERE id = @Id AND deleted_at IS NULL",
                connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? ReadDto(reader) : null;
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan offerte niet ophalen.", ex);
        }
    }

    public int Add(CreateQuoteDto dto)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"INSERT INTO offerte (travelagent_id, titel, taal, status, created_at, updated_at)
                  OUTPUT INSERTED.id
                  VALUES (@TravelAgentId, @Title, @Language, @Status, SYSUTCDATETIME(), SYSUTCDATETIME())",
                connection);
            command.Parameters.AddWithValue("@TravelAgentId", dto.TravelAgentId);
            command.Parameters.AddWithValue("@Title", dto.Title);
            command.Parameters.AddWithValue("@Language", dto.Language);
            command.Parameters.AddWithValue("@Status", dto.Status);
            connection.Open();
            var result = command.ExecuteScalar();
            return result is null ? 0 : Convert.ToInt32(result);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan offerte niet opslaan.", ex);
        }
    }

    public void Update(int id, CreateQuoteDto dto)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"UPDATE offerte
                  SET titel = @Title, taal = @Language, status = @Status, updated_at = SYSUTCDATETIME()
                  WHERE id = @Id AND deleted_at IS NULL",
                connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Title", dto.Title);
            command.Parameters.AddWithValue("@Language", dto.Language);
            command.Parameters.AddWithValue("@Status", dto.Status);
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan offerte niet bijwerken.", ex);
        }
    }

    public void SoftDelete(int id)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"UPDATE offerte
                  SET deleted_at = SYSUTCDATETIME()
                  WHERE id = @Id AND deleted_at IS NULL",
                connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan offerte niet verwijderen.", ex);
        }
    }

    private static QuoteDto ReadDto(IDataReader reader)
    {
        return new QuoteDto
        {
            Id = reader.GetInt32(0),
            TravelAgentId = reader.GetInt32(1),
            Title = reader.GetString(2),
            Language = reader.GetString(3),
            Status = reader.GetString(4),
            CreatedAt = reader.GetDateTime(5),
            UpdatedAt = reader.GetDateTime(6)
        };
    }
}
