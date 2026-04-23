using DAL;
using Interface;
using Logic;
using Interface.DTO;
using Microsoft.AspNetCore.Mvc;

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


        //lijst tonen
        public IActionResult Index()
        {
            // We maken hier handmatig een lijst aan. 
            // Als de website nu nog steeds 'null' zegt, dan draait er ECHT iets ouds.
            var quotes = new List<Interface.DTO.QuoteDTO>
    {
        new Interface.DTO.QuoteDTO { Title = "GELUKT!", Language = "NL", Status = "Test" }
    };

            return View(quotes);
        }


        //lijst aanmaken

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(QuoteDTO quoteDto) 
        {
            try
            {
                _quoteCollection.CreateNewQuote(quoteDto);
                return RedirectToAction("Index"); // Ga terug naar het overzicht na succes
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(quoteDto); // Toon het formulier opnieuw bij een fout
            }
        }
        

    }
}
