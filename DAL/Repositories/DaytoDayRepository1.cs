using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using DAL.DTO;

namespace DAL.Repositories
{
    public class DaytoDayRepository1
    {
        private readonly string _connectionString;
        public DaytoDayRepository1(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }
        public DaytoDayDTO? GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT * FROM Day WHERE Id = @id";
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
                string query = @"UPDATE Day SET Title = @title, Description = @description, Updated_At = @updatedAt WHERE Id = @id";
                using SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@title", title);
                command.Parameters.AddWithValue("@description", description);
                command.Parameters.AddWithValue("@updatedAt", DateTime.Now);
                command.ExecuteNonQuery();
            }
        }
        public DaytoDayDTO Insert(DaytoDayDTO item)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO Day (DayNumber, Title, Description, Created_At) VALUES (@dayNumber, @title, @description, @createdAt); SELECT SCOPE_IDENTITY();";
                using SqlCommand command = new SqlCommand(query, conn);
                command.Parameters.AddWithValue("@dayNumber", item.DayNumber);
                command.Parameters.AddWithValue("@title", item.Title);
                command.Parameters.AddWithValue("@description", item.Description);
                command.Parameters.AddWithValue("@createdAt", DateTime.Now);
                int newId = Convert.ToInt32(command.ExecuteScalar());
                return new DaytoDayDTO(newId, item.DayNumber, item.Title, item.Description);


            }
        }
    }
}

        
    
