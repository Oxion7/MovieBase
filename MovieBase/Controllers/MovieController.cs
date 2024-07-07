using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieBase.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.Diagnostics;

namespace MovieBase.Controllers
{
    public class MovieController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly MovieContext _db;
        private readonly UserManager<User> _userManager;
        private const int ImageWidth = 150;
        private const int ImageHeight = 200;

        public MovieController(IWebHostEnvironment env, MovieContext context, UserManager<User> userManager)
        {
            _environment = env;
            _db = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var movies = GetAllMovies();
            return View(movies);
        }

        public IActionResult Search(string searchStr)
        {
            var movies = GetAllMovies();
            if (string.IsNullOrEmpty(searchStr))
            {
                ViewBag.Msg = "Напишите в строке поиска название фильма, что вы ищете.";
                return View("Index", movies);
            }

            var searchResults = movies.Where(m =>
                m.Name.Contains(searchStr, StringComparison.OrdinalIgnoreCase) ||
                m.Genre.Name.Contains(searchStr, StringComparison.OrdinalIgnoreCase) ||
                m.ReleaseYear.ToString().Contains(searchStr, StringComparison.OrdinalIgnoreCase) ||
                m.Country.Contains(searchStr, StringComparison.OrdinalIgnoreCase)).ToList();

            ViewBag.Msg = searchResults.Count == 0
                ? "По Вашему запросу ничего не найдено"
                : $"По Вашему запросу найдено: {searchResults.Count} фильмов";

            return View("Index", searchResults.Any() ? searchResults : movies);
        }

        [Authorize(Roles = "manager")]
        public IActionResult Create()
        {
            ViewBag.Genre = new SelectList(_db.Genres, "Id", "Name");
            return View();
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        public IActionResult Create(Movie movie, IFormFile upload)
        {
            if (upload != null)
            {
                movie.ImageUrl = SaveImage(upload);
            }
            _db.Movies.Add(movie);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();

            var movie = GetMovieById(id.Value);
            if (movie == null)
                return NotFound();

            return View(movie);
        }

        [Authorize(Roles = "manager")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var movie = GetMovieById(id.Value);
            if (movie == null)
                return NotFound();

            ViewBag.Genre = new SelectList(_db.Genres, "Id", "Name");
            return View(movie);
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        public IActionResult Edit(Movie movie, IFormFile upload)
        {
            if (upload != null)
            {
                movie.ImageUrl = SaveImage(upload);
            }
            _db.Movies.Update(movie);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "manager")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var movie = GetMovieById(id.Value);
            if (movie == null)
                return NotFound();

            return View(movie);
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var movie = GetMovieById(id);
            if (movie != null)
            {
                _db.Movies.Remove(movie);
                _db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorViewModel = new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
            return View(errorViewModel);
        }

        private List<Movie> GetAllMovies()
        {
            return _db.Movies.Include(m => m.Genre).ToList();
        }

        private Movie? GetMovieById(int id)
        {
            return _db.Movies.Include(m => m.Genre).FirstOrDefault(m => m.Id == id);
        }

        private string SaveImage(IFormFile upload)
        {
            string fileName = Path.GetFileName(upload.FileName);
            var extFile = fileName.Substring(fileName.LastIndexOf('.'));
            if (new[] { ".png", ".bmp", ".jpg", ".jpeg" }.Contains(extFile.ToLower()))
            {
                var image = Image.Load(upload.OpenReadStream());
                image.Mutate(x => x.Resize(ImageWidth, ImageHeight));
                string path = Path.Combine(_environment.WebRootPath, "images", fileName);
                image.Save(path);
                return fileName;
            }
            return null;
        }
    }
}