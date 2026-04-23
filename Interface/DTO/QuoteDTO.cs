using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.DTO
{
    public class QuoteDTO
    {
        public int Id { get; set; }
        public int TravelAgentId { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
