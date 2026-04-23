using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interface;
using Logic.Models;
using Interface.DTO;

namespace Logic
{
    public class QuoteCollection
    {
        private readonly IQuoteContext _context;

        public QuoteCollection(IQuoteContext context)
        {
            _context = context;
        }

        //ophalen lijst


        public List<QuoteDTO> GetQuotes()
        {
            var list = _context.GetAll();
            return list ?? new List<QuoteDTO>();
            //return _context.GetAll();
        }

        public int CreateNewQuote(QuoteDTO quote)
        {
            if (string.IsNullOrWhiteSpace(quote.Title))
                throw new ArgumentException("Titel is required.");

            quote.Status = "concept";
            quote.TravelAgentId = 1;

            return _context.Insert(quote);


        }
    }
}
