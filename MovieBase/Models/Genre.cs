using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MovieBase.Models
{
    //TODO: CHANGE TO MANAGER
    [Table("Genre")]
    public class Genre
    {
        [HiddenInput(DisplayValue = false)]
        [Key]
        public int Id { get; set; } // ID

        [Required(ErrorMessage = "Пожалуйста введите название жанра")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        [Display(Name = "Жанр")]
        public string Name { get; set; } // название

        [HiddenInput(DisplayValue = true)]
        public IEnumerable<Movie> Movies { get; set; }
    }
}