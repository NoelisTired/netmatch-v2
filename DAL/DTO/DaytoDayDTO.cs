using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic.Models;

namespace DAL.DTO
{
    public class DaytoDayDTO
    {
        public int Id {  get; set; }
        public int DayNumber { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        public DateTime Created_At { get; private set; } = DateTime.Now;

        public DateTime Updated_At { get; private set;  } = DateTime.Now;
        public DateTime Deleted_At { get; private set; } = DateTime.Now;

        public DaytoDayDTO(int id, int dayNumber, string title, string description)
        {
            this.Id = id;
            this.DayNumber = dayNumber;
            this.Title = title;
            this.Description = description;
        }

    }
}
