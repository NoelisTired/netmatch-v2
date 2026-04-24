using DAL;
using Interface;
using Logic;
using Interface.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NetMatch.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Presentation.Controllers
{
    public class QuoteController : Controller
    {
        private readonly QuoteCollection _quoteCollection;

        public QuoteController(IConfiguration configuration)
        {
            var dbConn = new DatabaseConnection(configuration);
            IQuoteContext repo = new QuoteRepository(dbConn);
            _quoteCollection = new QuoteCollection(repo);
        }


       
        public IActionResult Index()
        {
            
            var quotes = _quoteCollection.GetQuotes();

          
            return View(quotes);
        }

        

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        public IActionResult Create(Interface.DTO.QuoteDTO newQuote)
        {
         
            _quoteCollection.AddQuote(newQuote);

            
            return RedirectToAction("Index");
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Edit(int id)
        {
            return View();
        }

        public IActionResult Store(int id)
        {
            
            return View();
        }
    }
}