using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UtulkyApp.Models;

namespace UtulkyApp.Controllers
{
    public class DogController : Controller
    {
        private readonly AppDbContext _db;

        public DogController(AppDbContext db)
        {
            _db = db;
        }

        // GET: Detail psa
        public async Task<IActionResult> Detail(int id)
        {
            var dog = await _db.Dogs.Include(d => d.Shelter).FirstOrDefaultAsync(d => d.Id == id);
            if (dog == null) return NotFound();
            return View(dog);
        }

        // GET: Formulář pro přidání psa
        [HttpGet]
        public IActionResult Create(int shelterId)
        {
            ViewBag.ShelterId = shelterId;
            return View();
        }

        // POST: Uložení psa
        [HttpPost]
        public async Task<IActionResult> Create(Dog dog)
        {
            if (ModelState.IsValid)
            {
                _db.Dogs.Add(dog);
                await _db.SaveChangesAsync();
                return RedirectToAction("Detail", "Shelter", new { id = dog.ShelterId });
            }
            ViewBag.ShelterId = dog.ShelterId;
            return View(dog);
        }

        // POST: Adopce (smazání)
        [HttpPost]
        public async Task<IActionResult> Adopt(int id)
        {
            var dog = await _db.Dogs.FindAsync(id);
            if (dog != null)
            {
                var shelterId = dog.ShelterId;
                _db.Dogs.Remove(dog);
                await _db.SaveChangesAsync();
                return RedirectToAction("Detail", "Shelter", new { id = shelterId });
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Vyhledávání
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View(new List<Dog>());
            }

            var dogs = await _db.Dogs
                .Include(d => d.Shelter) // Bonus: aby šlo prokliknout do útulku
                .Where(d => d.Name.ToLower().Contains(query.ToLower()))
                .ToListAsync();

            ViewBag.Query = query;
            return View(dogs);
        }
    }
}