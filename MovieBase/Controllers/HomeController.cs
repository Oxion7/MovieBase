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