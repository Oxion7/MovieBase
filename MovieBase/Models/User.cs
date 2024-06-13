using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MovieBase.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("User")]
    public class User : IdentityUser
    {
        public ICollection<UserMovie> UserMovies { get; set; }
    }
}