using System.ComponentModel.DataAnnotations;

namespace MovieBase.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Необходимо указать почту")]
        [EmailAddress]
        public string Email { get; set; }
    }
}