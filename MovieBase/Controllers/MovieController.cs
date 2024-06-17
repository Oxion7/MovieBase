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
        private IWebHostEnvironment _environment;
        private MovieContext _db;
        private UserManager<User> _userManager;
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
            var movies = _db.Movies.Include(m => m.Genre).ToList();
            return View(movies);
        }

        public IActionResult Search(string searchStr)
        {
            var movies = _db.Movies.Include(b => b.Genre).ToList();

            if (string.IsNullOrEmpty(searchStr))
            {
                ViewBag.Msg = "Напишите в строке поиска название фильма, что вы ищете.";
                return View("Index", movies);
            }

            var list = movies.Where(b =>
                b.Name.Contains(searchStr, StringComparison.OrdinalIgnoreCase) ||
                b.Genre.Name.Contains(searchStr, StringComparison.OrdinalIgnoreCase) ||
                b.ReleaseYear.ToString().Contains(searchStr, StringComparison.OrdinalIgnoreCase) ||
                b.Country.Contains(searchStr, StringComparison.OrdinalIgnoreCase)).ToList();

            if (list.Count == 0)
            {
                ViewBag.Msg = "По Вашему запросу ничего не найдено";
                return View("Index", movies);
            }
            else
            {
                ViewBag.Msg = $"По Вашему запросу найдено: {list.Count} фильмов";
                return View("Index", list);
            }
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
                string fileName = Path.GetFileName(upload.FileName);
                var extFile = fileName.Substring(fileName.LastIndexOf('.'));
                if (extFile.Contains("png") || extFile.Contains("bmp") || extFile.Contains("jpg")
                    || extFile.Contains("jpeg"))
                {
                    var image = Image.Load(upload.OpenReadStream());
                    image.Mutate(x => x.Resize(ImageWidth, ImageHeight));
                    string path = "\\wwwroot\\images\\" + fileName;
                    var hostPath = _environment.ContentRootPath + path;
                    image.Save(hostPath);
                    movie.ImageUrl = fileName;
                }
            }
            _db.Movies.Add(movie);
            _db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
                return NotFound();
            var movie = _db.Movies.Include(b => b.Genre).FirstOrDefault(b => b.Id == id);
            if (movie == null)
                return NotFound();
            return View(movie);
        }

        [Authorize(Roles = "manager")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();
            var movie = _db.Movies.Include(b => b.Genre).FirstOrDefault(b => b.Id == id);
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
                string fileName = Path.GetFileName(upload.FileName);
                var extFile = fileName.Substring(fileName.LastIndexOf('.'));
                if (extFile.Contains("png") || extFile.Contains("bmp") || extFile.Contains("jpg")
                    || extFile.Contains("jpeg"))
                {
                    var image = Image.Load(upload.OpenReadStream());
                    image.Mutate(x => x.Resize(ImageWidth, ImageHeight));
                    string path = "\\wwwroot\\images\\" + fileName;
                    var hostPath = _environment.ContentRootPath + path;
                    image.Save(hostPath);
                    movie.ImageUrl = fileName;
                }
            }
            _db.Movies.Add(movie).State = EntityState.Modified;
            _db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "manager")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var movie = _db.Movies.Include(b => b.Genre).FirstOrDefault(b => b.Id == id);
            if (movie == null)
                return NotFound();
            return View(movie);
        }

        [Authorize(Roles = "manager")]
        [HttpPost]
        public IActionResult Delete(Movie movie)
        {
            if (movie != null)
            {
                _db.Entry(movie).State = EntityState.Deleted;
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}