using System;
using System.Diagnostics.SymbolStore;

namespace Interface.DTO
{
    public class QuoteDTO
    {
        public int Id { get; set; }
        public int TravelAgentId { get; set; }
        public string? Title { get; set; } // NULL handling moet nog in service plaatsvinden
        public string? Language { get; set; } // parse naar enum in service (logic), NULL handling moet nog in service plaatsvinden
        public string? Status { get; set; } // parse naar enum in service (logic), NULL handling moet nog in service plaatsvinden
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime DeletedAt { get; set; } = DateTime.Now;
    }
}