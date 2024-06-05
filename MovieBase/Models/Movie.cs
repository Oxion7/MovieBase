using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieBase.Models
{
    [Table("Movie")]
    public class Movie
    {
        [HiddenInput(DisplayValue = false)]
        [Key]
        public int Id { get; set; } // ID

        [Required(ErrorMessage = "Пожалуйста введите название")]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 255 символов")]
        [Display(Name = "Название")]
        public string Name { get; set; } // название

        [Required]
        [Range(1700, 2024, ErrorMessage = "Недопустимый год")]
        [Display(Name = "Год выхода")]
        public int ReleaseYear { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        [Display(Name = "Страна")]
        public string Country { get; set; }
        [Required]
        [StringLength(250, MinimumLength = 30, ErrorMessage = "Длина строки должна быть от 30 до 250 символов")]
        [Display(Name = "Описание")]
        public string Description { get; set; }

        [HiddenInput(DisplayValue = false)]
        public int GenreId { get; set; }

        [Display(Name = "Жанр")]
        public Genre Genre { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string? ImageUrl { get; set; }
    }
}