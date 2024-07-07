using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBase.Models;

namespace MovieBase.Controllers
{
    [Authorize(Roles = "manager")]
    public class GenreController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly MovieContext _db;

        public GenreController(IWebHostEnvironment env, MovieContext context)
        {
            _environment = env;
            _db = context;
        }

        public IActionResult Index()
        {
            var genres = _db.Genres.ToList();
            return View(genres);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Genre genre)
        {
            if (GenreExists(genre.Name))
            {
                ModelState.AddModelError("Name", "Жанр с таким названием уже существует.");
                return View(genre);
            }

            _db.Genres.Add(genre);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var genre = _db.Genres.FirstOrDefault(g => g.Id == id);
            if (genre == null) return NotFound();

            var movieCount = _db.Movies.Count(m => m.GenreId == id);
            if (movieCount > 0)
            {
                ViewBag.MovieCount = movieCount;
                return View("DeleteConfirmation", genre);
            }

            return View(genre);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var genre = _db.Genres.Find(id);
            if (genre == null) return NotFound();

            _db.Genres.Remove(genre);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool GenreExists(string name)
        {
            return _db.Genres.Any(g => g.Name == name);
        }
    }
}