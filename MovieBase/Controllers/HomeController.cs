using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBase.Models;
using System.Diagnostics;

namespace MovieBase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MovieContext _db;

        public HomeController(ILogger<HomeController> logger, MovieContext context)
        {
            _logger = logger;
            _db = context;
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