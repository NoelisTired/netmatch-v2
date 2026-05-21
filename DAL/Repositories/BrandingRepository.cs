using System.Data;
using Interface.DataInterfaces;
using Interface.DTO;
using Microsoft.Data.SqlClient;

namespace DAL.Repositories;

/// <summary>
/// Datatoegang voor de huisstijl via ruwe SQL (tabel <c>branding</c>).
/// 1:1 met travelagent: <see cref="Save"/> doet een INSERT of UPDATE.
/// </summary>
public class BrandingRepository : IBrandingRepository
{
    private readonly string _connectionString;

    public BrandingRepository(string connectionString)
    {
        _connectionString = connectionString
            ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public BrandingDto? GetByTravelAgent(int travelAgentId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(
                @"SELECT id, travelagent_id, logo_pad, primaire_kleur, accent_kleur, created_at, updated_at
                  FROM branding
                  WHERE travelagent_id = @TravelAgentId AND deleted_at IS NULL",
                connection);
            command.Parameters.AddWithValue("@TravelAgentId", travelAgentId);
            connection.Open();
            using var reader = command.ExecuteReader();
            return reader.Read() ? ReadDto(reader) : null;
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan huisstijl niet ophalen.", ex);
        }
    }

    public void Save(CreateBrandingDto dto)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            // INSERT als er nog geen (niet-verwijderde) rij is, anders UPDATE.
            using var command = new SqlCommand(
                @"IF EXISTS (SELECT 1 FROM branding WHERE travelagent_id = @TravelAgentId AND deleted_at IS NULL)
                      UPDATE branding
                      SET logo_pad = @LogoPath, primaire_kleur = @PrimaryColor,
                          accent_kleur = @AccentColor, updated_at = SYSUTCDATETIME()
                      WHERE travelagent_id = @TravelAgentId AND deleted_at IS NULL;
                  ELSE
                      INSERT INTO branding (travelagent_id, logo_pad, primaire_kleur, accent_kleur, created_at, updated_at)
                      VALUES (@TravelAgentId, @LogoPath, @PrimaryColor, @AccentColor, SYSUTCDATETIME(), SYSUTCDATETIME());",
                connection);
            command.Parameters.AddWithValue("@TravelAgentId", dto.TravelAgentId);
            command.Parameters.AddWithValue("@LogoPath", (object?)dto.LogoPath ?? DBNull.Value);
            command.Parameters.AddWithValue("@PrimaryColor", dto.PrimaryColor);
            command.Parameters.AddWithValue("@AccentColor", dto.AccentColor);
            connection.Open();
            command.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            throw new InvalidOperationException("Kan huisstijl niet opslaan.", ex);
        }
    }

    private static BrandingDto ReadDto(IDataReader reader)
    {
        return new BrandingDto
        {
            Id = reader.GetInt32(0),
            TravelAgentId = reader.GetInt32(1),
            LogoPath = reader.IsDBNull(2) ? null : reader.GetString(2),
            PrimaryColor = reader.GetString(3),
            AccentColor = reader.GetString(4),
            CreatedAt = reader.GetDateTime(5),
            UpdatedAt = reader.GetDateTime(6)
        };
    }
}
