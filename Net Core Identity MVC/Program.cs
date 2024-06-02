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


// ���ݿ������ַ�����Ȼ�����ݿ�������ApplicationDbContextע�ᵽ����ע�������У�
// �����������ӵ�ָ����SqlServer���ݿ⡣���⣬�������һ�����ڿ�����Ա�쳣ҳ��Ĺ�������
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ��Identity������ӵ�DI������
builder.Services.AddIdentity<User, IdentityRole>()
    // ʹ��CustomIdentityErrorDescriber��Ϊ����������
    .AddErrorDescriber<CustomIdentityErrorDescriber>()
    // ��Entity Framework�洢��ӵ�Identity
    .AddEntityFrameworkStores<ApplicationDbContext>()
    // ���Ĭ�������ṩ����
    .AddDefaultTokenProviders();

builder.Services.AddRazorPages();
builder.Services.AddSession(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.IsEssential = true;
});
// ���GitHub OAuth2��֤
builder.Services.Configure<GithubCredential>(builder.Configuration.GetSection("Credentials:Github"));
builder.Services.Configure<GoogleCredential>(builder.Configuration.GetSection("Credentials:Google"));
// ���AutoMapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());





builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    //RequireDigit, RequireLowercase, RequireNonAlphanumeric, RequireUppercase, RequiredLength, RequiredUniqueChars�ֱ����������Ҫ��
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // �����û���¼��Ҫȷ�ϵ����ʼ�
    options.SignIn.RequireConfirmedEmail = true;

    //DefaultLockoutTimeSpan: �����˻�������Ĭ��ʱ���ȡ��ڴ�ʾ���У�����Ϊ5���ӡ�����ζ�ŵ��û����˻����ڵ�¼ʧ�ܶ�������ʱ��������5���Ӻ��Զ�������
    //MaxFailedAccessAttempts: ȷ���û����˻�������֮ǰ���������¼ʧ�ܳ��Դ��������������Ϊ5�Ρ�����ζ���û�������5�ε�¼ʧ�ܺ����˻�����������
    //AllowedForNewUsers: ȷ�����û��Ƿ���ܵ��˻��������Ե�Ӱ�졣�ڴ˴�����Ϊ true����ʾ���û�Ҳ���ܵ��������������Լ����
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // AllowedUserNameCharacters: ���������û������ַ������ڴ�ʾ���У�����ʹ�õ��ַ�������Сд��ĸ�������Լ�һЩ�����ַ�
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    //RequireUniqueEmail: ȷ���Ƿ�Ҫ��Ψһ�ĵ����ʼ���ַ�����û��˻����ڴ˴�����Ϊ false����ʾ��Ҫ��ÿ���û��ĵ����ʼ���ַ��Ψһ�ġ�
    options.User.RequireUniqueEmail = false;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // ConfigureApplicationCookie()������Ӧ�ó����cookie��֤��
    //HttpOnly����Ϊtrue��ȷ��cookie��ͨ��HTTP���͡�
    //ExpireTimeSpan����Ϊ5���Ӻ���ڡ�

    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    //LoginPath��AccessDeniedPath�ֱ�ָ���˵�¼ҳ��ͷ��ʱ��ܾ�ҳ���·����
    //SlidingExpiration����Ϊtrue�����û������ڡ�
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
});
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();

// ���Email����
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
