using Microsoft.AspNetCore.Identity;

namespace Net_Core_Identity_MVC.Entitys
{
    public class User: IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // 添加一个新的属性来存储用户的路径
        public string UserPath { get; set; }
        // 添加一个新的属性来存储用户头像的 URL
        public string AvatarUrl { get; set; }

        // 添加一个新的属性来存储用户注册时间
        public DateTime RegistrationTimestamp { get; set; }
    }
}
