using System.ComponentModel.DataAnnotations;

namespace MovieBase.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
