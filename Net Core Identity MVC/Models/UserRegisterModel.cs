using System.ComponentModel.DataAnnotations;

namespace Net_Core_Identity_MVC.Models
{
    public class UserRegisterModel
    {
        [Display(Name = "姓")]
        public string FirstName { get; set; }

        [Display(Name = "名")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "請輸入Email")]
        [EmailAddress(ErrorMessage = "Email格式不正確")]
        public string Email { get; set; }

        [Display(Name = "密碼")]  
        [Required(ErrorMessage = "請輸入密碼")]
        [StringLength(100, ErrorMessage = "密碼長度必須在 {2} 到 {1} 個字元", MinimumLength = 6)]
        public string Password { get; set; }

        [Display(Name = "確認密碼")]

        [Compare("Password", ErrorMessage = "密碼不一致")]
        public string ConfirmPassword { get; set; }
    }
}
