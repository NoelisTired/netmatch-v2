using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interface.DTO;

namespace Interface
{
    public interface IQuoteCollection
    {
        List<QuoteDTO> GetQuotes();
        void AddQuote(Interface.DTO.QuoteDTO quote);
    }
}
