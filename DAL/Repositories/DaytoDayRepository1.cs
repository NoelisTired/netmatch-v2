using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using DAL.DTO;

namespace DAL.Repositories
{
    public class DaytoDayRepository
    {
        private readonly string _connectionString;
        public DaytoDayRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }
        public DaytoDayDTO? GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT * FROM `day` WHERE id = @id";
                using SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@id", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new DaytoDayDTO
                        (
                            id: (int)reader["Id"],
                            dayNumber: (int)reader["DayNumber"],
                            title: reader["Title"].ToString(),
                            description: reader["Description"].ToString()
                        );
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
                string query = @"DELETE FROM Day WHERE Id = @Id";
                using SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
        }


        public void UpdateByID(int id, string title, string description, int dayNumber)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"UPDATE `day` SET Title = @title, description = @description, updated_at = @updatedAt WHERE id = @id";
                using SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@updatedAt", DateTime.Now);
                command.ExecuteNonQuery();
            }
        }
        public DaytoDayDTO Insert(DaytoDayDTO DTO)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO `day` (daynumber, title, description, created_At) VALUES (@dayNumber, @title, @description, @createdAt); 
`                SELECT SCOPE_IDENTITY();";

                using SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@dayNumber", DTO.DayNumber);
                command.Parameters.AddWithValue("@title", DTO.Title);
                command.Parameters.AddWithValue("@description", DTO.Description);
                command.Parameters.AddWithValue("@createdAt", DateTime.Now);
                command.ExecuteNonQuery();
                return DTO;
            }
        }
    }
}

        
    
