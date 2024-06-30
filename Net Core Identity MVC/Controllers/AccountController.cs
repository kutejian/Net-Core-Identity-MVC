using AutoMapper;
using EmailServer.Model;
using EmailServer.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Net_Core_Identity_MVC.Entitys;
using Net_Core_Identity_MVC.Models;
using System.Security.Claims;

namespace Net_Core_Identity_MVC.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<User> _signInManager;
        private readonly EmailServer.Server.IEmailSender _emailSender;
        public AccountController(UserManager<User> userManager, IMapper mapper , SignInManager<User> signInManager, EmailServer.Server.IEmailSender emailSender)
        {
            _userManager = userManager;
            _mapper = mapper;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        //防止跨域请求            
        [ValidateAntiForgeryToken]
        public IActionResult Register(UserRegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // 将model映射为User对象
                var user = _mapper.Map<User>(model);
                user.AvatarUrl = "aa";
                user.UserPath = "bb";
                user.RegistrationTimestamp=DateTime.Now;
                // 创建用户并设置密码，同步等待结果
                var result = _userManager.CreateAsync(user, model.Password).Result;
                if (result.Succeeded)
                {
                    // 发送验证邮件
                    var token = _userManager.GenerateEmailConfirmationTokenAsync(user).Result;
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { token, email = user.Email }, Request.Scheme);
                    //TODO: 发送邮件
                    var message = new EmailMessage(user.FirstName, user.Email, "Confirm your email",
                        $"Please confirm your email by clicking the following link: <a href='{callbackUrl}'>Confirm Email</a>");
                    _emailSender.SendEmailAsync(message);

                    // 若创建成功，将用户添加到"User"角色中，同步等待操作完成，然后重定向到"Login"页面
                    _userManager.AddToRoleAsync(user, "User").Wait();
                    return RedirectToAction("LoginConfirmation");
                }
                // 若创建失败，遍历错误列表，并将每个错误信息添加到模型状态中
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

            }
            return View();
        }
        [HttpGet]
        public IActionResult ConfirmEmail(string token, string email)
        {
            var user = _userManager.FindByEmailAsync(email).Result;
            if (user != null)
            {
                var result = _userManager.ConfirmEmailAsync(user, token).Result;
                if (result.Succeeded)
                {
                    return View("ConfirmEmail");
                }
                else
                {
                    return View("Error");
                }
            }
            return View("Error");
        }
        [HttpGet]
        public IActionResult LoginConfirmation()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        //防止跨域请求            
        [ValidateAntiForgeryToken]
        public IActionResult Login(UserLoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.FindByNameAsync(model.Email).Result;
                if (user != null && _userManager.CheckPasswordAsync(user, model.Password).Result 
                    && _userManager.IsEmailConfirmedAsync(user).Result )
                {
                    //var result = _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: false).Result;

                    var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                    identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
                    identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));
                    identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                    var  rols = _userManager.GetRolesAsync(user).Result;
                    foreach (var role in rols)
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }

                    HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));
                    return RedirectToAction("Employee", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View();
        }
        public IActionResult LoginOut()
        {
            _signInManager.SignOutAsync().Wait();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            var w = _signInManager.IsSignedIn(User);
            var isLoggedIn2 =HttpContext.User.Identity.IsAuthenticated;
            return View();
        }
        [HttpPost]
        //防止跨域请求            
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordModel model)
        {
            
            if (ModelState.IsValid)
            {
                var user = _userManager.FindByEmailAsync(model.Email).Result;
                if (user != null)
                {
                    //生成重置密码令牌并同步等待结果
                    var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;
                    var callbackUrl = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, protocol: HttpContext.Request.Scheme);
                    //TODO: 发送邮件
                    var message = new EmailMessage($"{user.FirstName}{user.LastName}",user.Email,"重置密码",
                        $"Please reset your password by clicking the following link: <a href='{callbackUrl}'>Reset Password</a>");

                    _emailSender.SendEmailAsync(message);

                    return View("ForgotPasswordConfirmation");
                }
                ModelState.AddModelError(string.Empty, "Invalid email.");
            }
            return View(model);
        }

        public IActionResult ForgotPasswordConfirmation() 
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            var model = new ResetPasswordModel { Token = token, Email = email };
            return View(model);
        }
        [HttpPost]
        //防止跨域请求            
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userManager.FindByEmailAsync(model.Email).Result;
                if (user != null)
                {
                    var result = _userManager.ResetPasswordAsync(user, model.Token, model.Password).Result;
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                }
            }
            return View(model);
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
