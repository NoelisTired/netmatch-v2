using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Interface.DTO;

namespace DAL.Repositories
{
    public class QuoteRepository
    {
        private readonly string _connectionString;

        public QuoteRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public QuoteDTO? GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"SELECT * FROM offerte WHERE id = @id";

                using SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@id", id);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new QuoteDTO
                        {
                            Id = (int)reader["id"],
                            TravelAgentId = (int)reader["travelagent_id"],
                            Title = reader["titel"].ToString(),
                            Language = reader["taal"].ToString(),
                            Status = reader["status"].ToString() 
                        };
                    }
                }
            }

            return null;
        }

        public void RemoveByID(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"DELETE FROM Offerte WHERE id = @Id";

                using SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@id", id);

                command.ExecuteNonQuery();
            }
        }

        public void UpdateByID(int id, string titel, string taal, string status)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"UPDATE Offerte SET titel = @Titel, taal = @Taal, status = @Status WHERE id = @Id";

                using SqlCommand command = new SqlCommand(query, conn);
                
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Titel", titel);
                command.Parameters.AddWithValue("@Taal", taal);
                command.Parameters.AddWithValue("@Status", status);

                command.ExecuteNonQuery();
            }
        }

        public QuoteDTO Insert(QuoteDTO DTO)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO Offerte (travelagent_id, titel, taal, status) 
                                VALUES (@TravelAgentId, @Titel, @Taal, @Status)";

                using SqlCommand command = new SqlCommand(query, conn);

                command.Parameters.AddWithValue("@TravelAgentId", DTO.TravelAgentId);
                command.Parameters.AddWithValue("@Titel", DTO.Title);
                command.Parameters.AddWithValue("@Taal", DTO.Language);
                command.Parameters.AddWithValue("@Status", DTO.Status);

                command.ExecuteNonQuery();

                return DTO;
            }
        }
    }
}

