using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyWebApp.Data;
using MyWebApp.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MyWebAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyWebAppDbContext") ?? throw new InvalidOperationException("Connection string 'MyWebAppDbContext' not found.")));

//註冊DI
builder.Services.AddScoped<ProductService, ProductService>();
builder.Services.AddScoped<GoogleMapTestService, GoogleMapTestService>();

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        // 配置 CookieAuthenticationOptions
        //範例，設置了Cookie的名稱為"MyCookie"、存在跟目錄、過期時間為30分鐘、登入路徑為"/Member/Login"、登出路徑為"/Member/Logout"
        options.Cookie.Name = "MyWebAppCookie";
        options.Cookie.Path = "/";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = "/Members/Login";
        options.LogoutPath = "/Members/Logout";

        //從配置節中讀取配置的 Cookie 信息
        //options.LoginPath = Config["web:loginUrl"];
        //options.Cookie.Name = Config["web:cookieName"];
        //options.Cookie.Path = Config["web:cookiePath"];
        //options.Cookie.Domain = Config["web:cookieDomain"];
    });

//設定權限
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireClaim(System.Security.Claims.ClaimTypes.Role, "admin");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//驗證服務
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
