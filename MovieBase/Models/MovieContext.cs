using Microsoft.EntityFrameworkCore;

namespace MovieBase.Models
{
    public class MovieContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }

        public DbSet<MovieList> MovieList { get; set; }

        public DbSet<User> Users { get; set; }

        public MovieContext(DbContextOptions<MovieContext> options)
             : base(options)
        {
        }
    }
}