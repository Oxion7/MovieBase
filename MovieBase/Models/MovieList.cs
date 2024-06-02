using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MovieBase.Models
{
    [Table("MovieList")]
    public class MovieList
    {
        [Key]
        public int Id { get; set; } // ID

        public IEnumerable<Movie> Movies { get; set; }
    }
}