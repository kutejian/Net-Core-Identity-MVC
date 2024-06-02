using System.ComponentModel.DataAnnotations;

namespace Net_Core_Identity_MVC.Models
{
    public class ResetPasswordModel
    {
        [Display(Name = "密码")]
        [Required(ErrorMessage = "密码不能为空")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "确认密码")]
        [Required(ErrorMessage = "确认密码不能为空")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string Token  { get; set; }

        public string Email { get; set; }
    }
}
