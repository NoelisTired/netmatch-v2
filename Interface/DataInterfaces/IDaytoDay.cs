using Interface.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.DataInterfaces
{
    public interface IDaytoDay
    {
        DaytoDayDTO? GetById(int id);
        void RemoveByID(int id);
        void UpdateByID(int id, string title, string description, int dayNumber);
        DaytoDayDTO Insert(DaytoDayDTO DTO);
    }
}
