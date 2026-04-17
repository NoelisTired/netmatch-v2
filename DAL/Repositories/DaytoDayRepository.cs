using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using DAL.DTO;
using Interface.DataInterfaces;

namespace DAL.Repositories
{
    public class DaytoDayRepository
    {
        private readonly string _connectionString;

        public DaytoDayRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }
        public IReadOnlyList<DaytoDayDTO> GetAll()
        {
            var results = new List<DaytoDayDTO>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT Id, DayNumber, Title, Description FROM Day";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var id = reader.GetInt32(reader.GetOrdinal("Id"));
                var dayNumber = reader.IsDBNull(reader.GetOrdinal("DayNumber")) ? 0 : reader.GetInt32(reader.GetOrdinal("DayNumber"));
                var title = reader.IsDBNull(reader.GetOrdinal("Title")) ? string.Empty : reader.GetString(reader.GetOrdinal("Title"));
                var description = reader.IsDBNull(reader.GetOrdinal("Description")) ? string.Empty : reader.GetString(reader.GetOrdinal("Description"));

                results.Add(new DaytoDayDTO(id, dayNumber, title, description));
            }

            return results;
        }
        public int Insert(DaytoDayDTO item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"
        INSERT INTO Day (DayNumber, Title, Description, Created_At)
        VALUES (@dayNumber, @title, @description, @createdAt);
        SELECT CAST(SCOPE_IDENTITY() AS int);";

            var pDay = cmd.CreateParameter();
            pDay.ParameterName = "@dayNumber";
            pDay.DbType = DbType.Int32;
            pDay.Value = item.DayNumber;
            cmd.Parameters.Add(pDay);

            var pTitle = cmd.CreateParameter();
            pTitle.ParameterName = "@title";
            pTitle.DbType = DbType.String;
            pTitle.Value = item.Title ?? string.Empty;
            cmd.Parameters.Add(pTitle);

            var pDesc = cmd.CreateParameter();
            pDesc.ParameterName = "@description";
            pDesc.DbType = DbType.String;
            pDesc.Value = item.Description ?? string.Empty;
            cmd.Parameters.Add(pDesc);

            var pCreated = cmd.CreateParameter();
            pCreated.ParameterName = "@createdAt";
            pCreated.DbType = DbType.DateTime;
            pCreated.Value = item.Created_At;
            cmd.Parameters.Add(pCreated);

            var result = cmd.ExecuteScalar();
            return result == null ? 0 : Convert.ToInt32(result);
        }

        public bool Update(DaytoDayDTO item)
        {
            if (item is null) throw new ArgumentNullException(nameof(item));

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"UPDATE Day
                                SET DayNumber = @dayNumber,
                                    Title = @title,
                                    Description = @description
                                WHERE Id = @id";

            var pId = cmd.CreateParameter();
            pId.ParameterName = "@id";
            pId.DbType = DbType.Int32;
            pId.Value = item.Id;
            cmd.Parameters.Add(pId);

            var pDay = cmd.CreateParameter();
            pDay.ParameterName = "@dayNumber";
            pDay.DbType = DbType.Int32;
            pDay.Value = item.DayNumber;
            cmd.Parameters.Add(pDay);

            var pTitle = cmd.CreateParameter();
            pTitle.ParameterName = "@title";
            pTitle.DbType = DbType.String;
            pTitle.Value = item.Title ?? string.Empty;
            cmd.Parameters.Add(pTitle);

            var pDesc = cmd.CreateParameter();
            pDesc.ParameterName = "@description";
            pDesc.DbType = DbType.String;
            pDesc.Value = item.Description ?? string.Empty;
            cmd.Parameters.Add(pDesc);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }

        public bool Delete(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "DELETE FROM Day WHERE Id = @id";

            var p = cmd.CreateParameter();
            p.ParameterName = "@id";
            p.DbType = DbType.Int32;
            p.Value = id;
            cmd.Parameters.Add(p);

            var rows = cmd.ExecuteNonQuery();
            return rows > 0;
        }
    }
}
        

