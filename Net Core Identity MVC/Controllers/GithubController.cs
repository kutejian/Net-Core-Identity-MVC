using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Net_Core_Identity_MVC.Entitys;
using Net_Core_Identity_MVC.Models;
using Net_Core_Identity_MVC.Models.GitHubModel;
using Net_Core_Identity_MVC.Models.GitHubModel.ViewModels;
using Newtonsoft.Json;

using System.Net.Http.Headers;
using System.Security.Claims;



namespace OAuth20.Lab.Controllers
{
    public class GithubController : Controller
    {
        private const string AuthorizeUri = "https://github.com/login/oauth/authorize";

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public GithubController(IHttpClientFactory httpClientFactory, IOptions<GithubCredential> options,UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _httpClientFactory = httpClientFactory;

            var credential = options.Value;

            _clientId = credential.ClientId;
            _clientSecret = credential.ClientSecret;
            _redirectUri = credential.RedirectUri;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Authorize()
        {
            var scopes = new List<string>
            {
                "user","repo","gist"
            };
            
            var param = new Dictionary<string, string>
            {
                { "client_id", _clientId },
                { "redirect_uri", _redirectUri },
                { "scope", string.Join(',', scopes) },
                { "state", "123456" }
            };

            var requestUri = QueryHelpers.AddQueryString(AuthorizeUri, param);

            return Redirect(requestUri);
        }
        //根据 GitHub给的coed 生成一个token 并且储层到了Cookies里 根据cookies里拿
        public async Task<IActionResult> Callback(string code, string state)
        {
            const string uri = "https://github.com/login/oauth/access_token";

            var param = new Dictionary<string, string>
            {
                ["client_id"] = _clientId,
                ["client_secret"] = _clientSecret,
                ["code"] = code,
                ["state"] = state
            };
            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await httpClient.PostAsync(
                uri,
                new FormUrlEncodedContent(param));
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"{response.StatusCode}, {responseContent}");
            }
            string accessToken = Convert.ToString(
                JsonConvert.DeserializeObject<dynamic>(responseContent)!.access_token);

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException($"accessToken is empty");
            }
            HttpContext.Response.Cookies.Append(CookieNames.GithubAccessToken, accessToken);
            return RedirectToAction("UserData", "Github");
        }

        /// <summary>
        /// 用户已登录 获取到了 GitHub上传来的Token  
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> UserData()
        {
            const string uri = "https://api.github.com/user";

            using var httpClient = _httpClientFactory.CreateClient();

            if (!HttpContext.Request.Cookies.TryGetValue(CookieNames.GithubAccessToken, out var accessToken))
            {
                return RedirectToAction("Authorize");
            }
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Awesome-Octocat-App");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", accessToken);

            var response = await httpClient.GetAsync(uri);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"{response.StatusCode}, {responseContent}");
            }
            var gitHubUser = JsonConvert.DeserializeObject<GithubUserViewModel>(responseContent);

            var user = await _userManager.FindByEmailAsync(gitHubUser.Email);
            if (user == null)
            {
                // 如果用户不存在，则创建新用户
                user = new User
                {
                    UserName = gitHubUser.Email,
                    LastName = gitHubUser.Name,
                    FirstName = gitHubUser.Name,
                    Email = gitHubUser.Email,
                    AvatarUrl = gitHubUser.AvatarUrl,
                };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "User").Wait();
                    var usermodel = _userManager.FindByNameAsync(user.Email).Result;
                    GithubLogin(usermodel);
                    return RedirectToAction("Employee", "Home");
                }
            }
            GithubLogin(user);
            return RedirectToAction("Employee", "Home");
            return View(gitHubUser);
        }
        public void GithubLogin(User user)
        {

            var identity = new ClaimsIdentity(IdentityConstants.ApplicationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
            identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            var rols = _userManager.GetRolesAsync(user).Result;
            foreach (var role in rols)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            //这行代码实现了用户登录的功能，使用了ASP.NET Core的身份验证中间件，将用户标识信息添加到当前HTTP上下文中，并指示用户已成功登录。
            HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity)).Wait();
           
        }
    }
}