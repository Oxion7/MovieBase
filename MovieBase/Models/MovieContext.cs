using Microsoft.EntityFrameworkCore;
using MovieBase.ViewModels;

namespace MovieBase.Models
{
    public class MovieContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public DbSet<UserMovie> UserMovies { get; set; }

        public MovieContext(DbContextOptions<MovieContext> options)
             : base(options)
        {
        }

        public DbSet<MovieBase.ViewModels.UserViewModel>? UserViewModel { get; set; }
    }
}