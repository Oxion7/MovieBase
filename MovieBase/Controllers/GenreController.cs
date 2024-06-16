using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieBase.Models;

namespace MovieBase.Controllers
{
    public class GenreController : Controller
    {
        private IWebHostEnvironment _environment;
        private MovieContext _db;

        public GenreController(IWebHostEnvironment env, MovieContext context)
        {
            _environment = env;
            _db = context;
        }

        [Authorize(Roles = "manager")]
        public IActionResult Index()
        {
            var genre = _db.Genres.ToList();
            return View(genre);
        }

        [Authorize(Roles = "manager")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        public IActionResult Create(Genre genre)
        {
            _db.Genres.Add(genre);
            _db.SaveChanges();
            return RedirectToAction("Index", "Genre");
        }

        //TODO: сделать предупреждение если есть фильм этого жанра
        [Authorize(Roles = "manager")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var genre = _db.Genres.FirstOrDefault(b => b.Id == id);
            if (genre == null)
                return NotFound();
            return View(genre);
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        public IActionResult Delete(Genre genre)
        {
            if (genre != null)
            {
                _db.Entry(genre).State = EntityState.Deleted;
                _db.SaveChanges();
            }
            return RedirectToAction("Index", "Genre");
        }
    }
}