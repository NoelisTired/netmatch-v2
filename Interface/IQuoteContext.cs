using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interface.DTO;

namespace Interface
{
    public interface IQuoteContext
    {
        

        int Insert(QuoteDTO quote);

        List<QuoteDTO> GetAll();
    }
}
