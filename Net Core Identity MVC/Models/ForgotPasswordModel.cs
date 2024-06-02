using System.ComponentModel.DataAnnotations;

namespace Net_Core_Identity_MVC.Models
{
    public class ForgotPasswordModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
    }
}
