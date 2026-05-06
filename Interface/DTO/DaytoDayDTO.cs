using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Interface.DTO
{
    public class DaytoDayDTO
    {
        public int Id { get; set; }
        public int DayNumber { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public DateTime Created_At { get; set; } = DateTime.Now;

        public DateTime Updated_At { get; set; } = DateTime.Now;
        public DateTime Deleted_At { get; set; } = DateTime.Now;
    }
}
