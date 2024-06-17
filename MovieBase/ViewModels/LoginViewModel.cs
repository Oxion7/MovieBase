using System.ComponentModel.DataAnnotations;

namespace MovieBase.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Необходимо указать почту")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Необходимо указать пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}