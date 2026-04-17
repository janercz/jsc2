using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UtulkyApp.Models;

namespace UtulkyApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var shelters = await _db.Shelters.ToListAsync();
            return View(shelters);
        }
    }
}