using EmailServer.Model;
using EmailServer.Server;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Net_Core_Identity_MVC;
using Net_Core_Identity_MVC.Data;
using Net_Core_Identity_MVC.Entitys;
using Net_Core_Identity_MVC.Models.GitHubModel;
using Net_Core_Identity_MVC.Models.GoogleModel;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


// 数据库连接字符串，然后将数据库上下文ApplicationDbContext注册到依赖注入容器中，
// 并配置其连接到指定的SqlServer数据库。另外，还添加了一个用于开发人员异常页面的过滤器。
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 将Identity服务添加到DI容器中
builder.Services.AddIdentity<User, IdentityRole>()
    // 使用CustomIdentityErrorDescriber作为错误描述器
    .AddErrorDescriber<CustomIdentityErrorDescriber>()
    // 将Entity Framework存储添加到Identity
    .AddEntityFrameworkStores<ApplicationDbContext>()
    // 添加默认令牌提供程序
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();
builder.Services.AddSession(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.IsEssential = true;
});
// 添加GitHub OAuth2认证
builder.Services.Configure<GithubCredential>(builder.Configuration.GetSection("Credentials:Github"));
builder.Services.Configure<GoogleCredential>(builder.Configuration.GetSection("Credentials:Google"));
// 添加AutoMapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());





builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    //RequireDigit, RequireLowercase, RequireNonAlphanumeric, RequireUppercase, RequiredLength, RequiredUniqueChars分别设置密码的要求
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // 设置用户登录需要确认电子邮件
    options.SignIn.RequireConfirmedEmail = true;

    //DefaultLockoutTimeSpan: 设置账户锁定的默认时间跨度。在此示例中，设置为5分钟。这意味着当用户的账户由于登录失败而被锁定时，将会在5分钟后自动解锁。
    //MaxFailedAccessAttempts: 确定用户在账户被锁定之前允许的最大登录失败尝试次数。在这里，设置为5次。这意味着用户在连续5次登录失败后，其账户将被锁定。
    //AllowedForNewUsers: 确定新用户是否会受到账户锁定策略的影响。在此处设置为 true，表示新用户也会受到上述锁定规则的约束。
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // AllowedUserNameCharacters: 允许用作用户名的字符集。在此示例中，允许使用的字符包括大小写字母、数字以及一些特殊字符
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    //RequireUniqueEmail: 确定是否要求唯一的电子邮件地址用于用户账户。在此处设置为 false，表示不要求每个用户的电子邮件地址是唯一的。
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // ConfigureApplicationCookie()配置了应用程序的cookie认证。
    //HttpOnly设置为true以确保cookie仅通过HTTP发送。
    //ExpireTimeSpan设置为5分钟后过期。

    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    //LoginPath和AccessDeniedPath分别指定了登录页面和访问被拒绝页面的路径。
    //SlidingExpiration设置为true以启用滑动过期。
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();

// 添加Email服务
builder.Services.Configure<EmaliSendConfig>(builder.Configuration.GetSection("EmaliSendConfig"));
builder.Services.Configure<DataProtectionTokenProviderOptions>(
    options =>options.TokenLifespan = TimeSpan.FromHours(2));

builder.Services.AddScoped<IEmailSender, EmailSender>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
