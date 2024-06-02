using Microsoft.AspNetCore.Identity;

namespace Net_Core_Identity_MVC
{
    public class CustomIdentityErrorDescriber :IdentityErrorDescriber
    {
        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError
            {
                Code = nameof(PasswordTooShort),
                Description = $"密码至少需要{length}"
            };
        }   
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateEmail),
                Description = $"电子邮箱{email}已存在"
            };
        }
        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = nameof(DuplicateUserName),
                Description = $"用户名{userName}已存在"
            };
        }
    }
}
