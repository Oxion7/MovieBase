using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBase.Models;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace MovieBase.Controllers
{
    [Authorize]
    public class UserMovieController : Controller
    {
        private readonly MovieContext _db;
        private readonly UserManager<User> _userManager;

        public UserMovieController(MovieContext context, UserManager<User> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var movies = await GetMoviesForUser(user.Id);
            return View(movies);
        }

        [HttpGet]
        public async Task<IActionResult> AddMovieToUserAsync(int? id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            var movie = await _db.Movies.Include(b => b.Genre).FirstOrDefaultAsync(b => b.Id == id);
            if (movie == null) return NotFound();

            var userMovieExists = await _db.UserMovies.AnyAsync(um => um.UserId == user.Id && um.MovieId == movie.Id);
            if (userMovieExists) return RedirectToAction("MovieAlreadyExists", new { id = movie.Id });

            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> AddMovieToUser(Movie movie)
        {
            if (movie == null) return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var userMovie = new UserMovie { UserId = user.Id, MovieId = movie.Id };
            _db.UserMovies.Add(userMovie);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMovieFromUser(int? id)
        {
            if (id == null) return NotFound();

            var movie = await GetMovieByIdAsync(id.Value);
            if (movie == null) return NotFound();

            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMovieFromUser(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            var userMovie = await _db.UserMovies.FirstOrDefaultAsync(um => um.UserId == user.Id && um.MovieId == id);
            if (userMovie == null) return NotFound();

            _db.UserMovies.Remove(userMovie);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult MovieAlreadyExists(int? id)
        {
            if (id == null) return NotFound();

            var movie = _db.Movies.Include(b => b.Genre).FirstOrDefault(b => b.Id == id);
            if (movie == null) return NotFound();

            return View(movie);
        }

        private async Task<List<Movie>> GetMoviesForUser(string userId)
        {
            return await _db.UserMovies
                            .Include(um => um.Movie)
                            .ThenInclude(m => m.Genre)
                            .Where(um => um.UserId == userId)
                            .Select(um => um.Movie)
                            .ToListAsync();
        }

        private async Task<Movie?> GetMovieByIdAsync(int id)
        {
            return await _db.Movies.Include(b => b.Genre).FirstOrDefaultAsync(b => b.Id == id);
        }
    }
}