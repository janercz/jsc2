using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UtulkyApp.Models;

namespace UtulkyApp.Controllers
{
    public class ShelterController : Controller
    {
        private readonly AppDbContext _db;

        public ShelterController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Detail(int id)
        {
            var shelter = await _db.Shelters
                .Include(s => s.Dogs)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shelter == null) return NotFound();
            return View(shelter);
        }
    }
}