using System;
using System.Collections.Generic;
using Interface;
using Interface.DTO;

namespace Logic
{
    public class QuoteCollection : IQuoteCollection
    {
        
        private readonly IQuoteContext _context;

       
        public QuoteCollection(IQuoteContext context)
        {
            _context = context;
        }

        public List<QuoteDTO> GetQuotes()
        {
            var list = _context.GetAll();
            return list ?? new List<QuoteDTO>();
        }

       
        public void AddQuote(QuoteDTO quote)
        {
            
            quote.Status = "concept";

            
            _context.Insert(quote);
        }
    }
}