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
        private const int ImageWidth = 150;
        private const int ImageHeight = 200;

        public MovieController(IWebHostEnvironment env, MovieContext context)
        {
            _environment = env;
            _db = context;
        }

        public IActionResult Index()
        {
            var movies = _db.Movies.Include(m => m.Genre).ToList();
            return View(movies);
        }

        public IActionResult Create()
        {
            ViewBag.Genre = new SelectList(_db.Genres, "Id", "Name");
            return View();
        }

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

        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();
            var movie = _db.Movies.Include(b => b.Genre).FirstOrDefault(b => b.Id == id);
            if (movie == null)
                return NotFound();
            return View(movie);
        }

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