using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interface;
using Microsoft.Data.SqlClient;
using System.Data;
using Interface.DTO;

namespace DAL
{
    public class QuoteRepository : IQuoteContext
    {
        private readonly DatabaseConnection _db;

        public QuoteRepository(DatabaseConnection db) 
        {
            _db = db;
        }

       

        public int Insert(QuoteDTO quote)
        {
            using (var conn = _db.GetConnection())
            {
                string query = @"INSERT INTO offerte (titel, taal, status, travelagent_id) VALUES(@title, @lang, @status, @agentId); SELECT SCOPE_IDENTITY();";

                using (var cmd = new Microsoft.Data.SqlClient.SqlCommand(query, (Microsoft.Data.SqlClient.SqlConnection)conn))
                {
                    cmd.Parameters.AddWithValue("@title", (object)quote.Title ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@lang", (object)quote.Language ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@status", "concept");
                    cmd.Parameters.AddWithValue("@agentId", 1);

                    conn.Open();
                    
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            } 
           
        }

        public List<QuoteDTO> GetAll()
        {
            List<QuoteDTO> quotes = new List<QuoteDTO>();

            try
            {
                using (var conn = _db.GetConnection())
                {
                    
                    string query = "SELECT id, travelagent_id, titel, taal, status FROM offerte";

                    using (var cmd = new Microsoft.Data.SqlClient.SqlCommand(query, (Microsoft.Data.SqlClient.SqlConnection)conn))
                    {
                        conn.Open(); 

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                quotes.Add(new QuoteDTO
                                {
                                    Id = reader.GetInt32(0),
                                    TravelAgentId = reader.GetInt32(1),
                                    Title = reader.GetString(2),
                                    Language = reader.GetString(3),
                                    Status = reader.GetString(4)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
             
                string foutmelding = ex.Message;
                throw;
            }
        
            return quotes;
        }
    }
}
