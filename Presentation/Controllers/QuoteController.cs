using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NetMatch.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
// Add appropriate using statements for IQuoteLogic and QuoteDTO if they are in a different namespace.

namespace NetMatch.Controllers;

public class QuoteController : Controller
{
    // 1. Inject your Logic Layer interface here
    private readonly IQuoteLogic _quoteLogic;

    public QuoteController(IQuoteLogic quoteLogic)
    {
        _quoteLogic = quoteLogic;
    }

    public IActionResult Index()
    {
        // 2. Fetch data from your Logic Layer
        var quoteDTOs = _quoteLogic.GetAllQuotes();
        
        // 3. Map the DTOs from the Logic Layer to presentation Models
        var quotes = new List<QuoteModel>();
        
        foreach (var dto in quoteDTOs)
        {
            quotes.Add(new QuoteModel 
            {
                Id = dto.Id,
                Titel = dto.Titel,
                Status = dto.Status,
                Taal = dto.Taal,
                Travelagent_id = dto.Travelagent_id
            });
        }

        // 4. Pass the mapped models to the view
        return View(quotes);
    }

    [HttpGet]
    public IActionResult Create() 
    {
        return View("CreateQuote");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(IFormCollection formData) 
    {
        // TODO: Add your logic to save the form data to the database here
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Show()
    {
        return View();
    }
    
    [HttpGet]
    [HttpPost]
    public IActionResult Edit(int id)
    {
        return View();
    }

    public IActionResult Store(int id)
    {
        // Edit route with id with data
        return View();
    }
}
