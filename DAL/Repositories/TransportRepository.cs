using System.Data;
using Interface.DataInterfaces;
using Interface.DTO;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories;

/// <summary>
/// Datatoegang voor transportmiddelen via ruwe SQL (tabel <c>transport</c>).
/// Alle queries filteren op <c>deleted_at IS NULL</c>. snake_case kolommen.
/// </summary>
public class TransportRepository : ITransportRepository
{
    private readonly string _connectionString;

    public TransportRepository(string connectionString)
    {
        _connectionString = connectionString
            ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public IEnumerable<TransportDto> GetByDay(int dayId)
    {
        var items = new List<TransportDto>();
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"SELECT id, dag_id, type, vertreklocatie, aankomstlocatie,
                         vluchtnummer, luchtvaartmaatschappij, prijs, created_at, updated_at
                  FROM transport
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
            throw new InvalidOperationException("Kan transportmiddelen niet ophalen.", ex);
        }
        return items;
    }

    public TransportDto? GetById(int id)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"SELECT id, dag_id, type, vertreklocatie, aankomstlocatie,
                         vluchtnummer, luchtvaartmaatschappij, prijs, created_at, updated_at
                  FROM transport
                  WHERE id = @Id AND deleted_at IS NULL",
                connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? ReadDto(reader) : null;
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan transport niet ophalen.", ex);
        }
    }

    public int Add(CreateTransportDto dto)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"INSERT INTO transport (dag_id, type, vertreklocatie, aankomstlocatie,
                                         vluchtnummer, luchtvaartmaatschappij, prijs, created_at, updated_at)
                  OUTPUT INSERTED.id
                  VALUES (@DayId, @Type, @Departure, @Arrival, @FlightNumber, @Airline, @Price,
                          SYSUTCDATETIME(), SYSUTCDATETIME())",
                connection);
            command.Parameters.AddWithValue("@DayId", dto.DayId);
            command.Parameters.AddWithValue("@Type", dto.Type);
            command.Parameters.AddWithValue("@Departure", dto.DepartureLocation);
            command.Parameters.AddWithValue("@Arrival", dto.ArrivalLocation);
            command.Parameters.AddWithValue("@FlightNumber", (object?)dto.FlightNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@Airline", (object?)dto.Airline ?? DBNull.Value);
            command.Parameters.AddWithValue("@Price", (object?)dto.Price ?? DBNull.Value);
            connection.Open();
            var result = command.ExecuteScalar();
            return result is null ? 0 : Convert.ToInt32(result);
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan transport niet opslaan.", ex);
        }
    }

    public void Update(int id, CreateTransportDto dto)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"UPDATE transport
                  SET type = @Type, vertreklocatie = @Departure, aankomstlocatie = @Arrival,
                      vluchtnummer = @FlightNumber, luchtvaartmaatschappij = @Airline,
                      prijs = @Price, updated_at = SYSUTCDATETIME()
                  WHERE id = @Id AND deleted_at IS NULL",
                connection);
            command.Parameters.AddWithValue("@Id", id);
            command.Parameters.AddWithValue("@Type", dto.Type);
            command.Parameters.AddWithValue("@Departure", dto.DepartureLocation);
            command.Parameters.AddWithValue("@Arrival", dto.ArrivalLocation);
            command.Parameters.AddWithValue("@FlightNumber", (object?)dto.FlightNumber ?? DBNull.Value);
            command.Parameters.AddWithValue("@Airline", (object?)dto.Airline ?? DBNull.Value);
            command.Parameters.AddWithValue("@Price", (object?)dto.Price ?? DBNull.Value);
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan transport niet bijwerken.", ex);
        }
    }

    public void SoftDelete(int id)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"UPDATE transport
                  SET deleted_at = SYSUTCDATETIME()
                  WHERE id = @Id AND deleted_at IS NULL",
                connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan transport niet verwijderen.", ex);
        }
    }

    private static TransportDto ReadDto(IDataReader reader)
    {
        return new TransportDto
        {
            Id = reader.GetInt32(0),
            DayId = reader.GetInt32(1),
            Type = reader.GetString(2),
            DepartureLocation = reader.GetString(3),
            ArrivalLocation = reader.GetString(4),
            FlightNumber = reader.IsDBNull(5) ? null : reader.GetString(5),
            Airline = reader.IsDBNull(6) ? null : reader.GetString(6),
            Price = reader.IsDBNull(7) ? null : reader.GetDecimal(7),
            CreatedAt = reader.GetDateTime(8),
            UpdatedAt = reader.GetDateTime(9)
        };
    }
}
