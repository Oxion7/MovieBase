using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MovieBase.Models
{
    [Table("UserMovie")]
    [PrimaryKey(nameof(UserId), nameof(MovieId))]
    public class UserMovie
    {
        public string UserId { get; set; }

        public User User { get; set; }

        public int MovieId { get; set; }

        public Movie Movie { get; set; }
    }
}