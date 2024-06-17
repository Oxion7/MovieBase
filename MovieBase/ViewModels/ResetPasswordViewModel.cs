using System.ComponentModel.DataAnnotations;

namespace MovieBase.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Необходимо указать почту")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Необходимо указать пароль")]
        [StringLength(100, ErrorMessage = "Пароль должен содержать как минимум 6 символов", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}