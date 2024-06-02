using Microsoft.EntityFrameworkCore;

namespace MovieBase.Models
{
    public class MovieContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public DbSet<MovieList> MoviesList { get; set; }

        public MovieContext(DbContextOptions<MovieContext> options)
             : base(options)
        {
        }
    }
}