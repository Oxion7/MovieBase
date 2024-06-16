using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieBase.Models;

namespace MovieBase.Controllers
{
    public class UserMovieController : Controller
    {
        private MovieContext _db;
        private UserManager<User> _userManager;
        //private RoleManager<IdentityRole> _roleManager;

        public UserMovieController(MovieContext context, UserManager<User> userManager)
        {
            _db = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch movies, potentially with some filtering logic for the specific user
            var movies = await GetMoviesForUser(user.Id);

            return View(movies);
        }

        [HttpGet]
        public async Task<IActionResult> AddMovieToUserAsync(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id == null)
                return NotFound();
            var movie = _db.Movies.Include(b => b.Genre).FirstOrDefault(b => b.Id == id);
            if (movie == null)
                return NotFound();
            var userMovieExists = await _db.UserMovies
                                    .AnyAsync(um => um.UserId == user.Id && um.MovieId == movie.Id);
            if (userMovieExists)
            {
                return RedirectToAction("MovieAlreadyExists", new { id = movie.Id });
            }
            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> AddMovieToUser(Movie movie)
        {
            if (movie == null)
                return NotFound();

            // Get the current logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // Create a new UserMovie entry
            var userMovie = new UserMovie
            {
                UserId = user.Id,
                MovieId = movie.Id
            };

            // Add the UserMovie entry to the database
            _db.UserMovies.Add(userMovie);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteMovieFromUser(int? id)
        {
            // Get the current logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            if (id == null)
                return NotFound();
            var movie = _db.Movies.Include(b => b.Genre).FirstOrDefault(b => b.Id == id);
            if (movie == null)
                return NotFound();
            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMovieFromUser(Movie movie)
        {
            // Get the current logged-in user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            // Find the UserMovie entry
            var userMovie = await _db.UserMovies
                                     .FirstOrDefaultAsync(um => um.UserId == user.Id && um.MovieId == movie.Id);
            if (userMovie == null)
                return NotFound();

            _db.UserMovies.Remove(userMovie);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult MovieAlreadyExists(int? id)
        {
            if (id == null)
                return NotFound();

            var movie = _db.Movies.Include(b => b.Genre).FirstOrDefault(b => b.Id == id);
            if (movie == null)
                return NotFound();

            return View(movie);
        }

        private async Task<List<Movie>> GetMoviesForUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new List<Movie>();
            }

            var movies = await _db.UserMovies
                                  .Include(um => um.Movie)
                                  .ThenInclude(m => m.Genre)
                                  .Where(um => um.UserId == userId)
                                  .Select(um => um.Movie)
                                  .ToListAsync();

            return movies;
        }
    }
}