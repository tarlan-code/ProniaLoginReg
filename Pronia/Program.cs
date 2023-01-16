using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Models;
using Pronia.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("MSSQL"));
});

builder.Services.AddScoped<LayoutService>();
builder.Services.AddSession();
builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
    opt.Password.RequireDigit = true;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 8;
    opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._";
    opt.User.RequireUniqueEmail = true;
    opt.Lockout.AllowedForNewUsers = true;


}).AddDefaultTokenProviders().AddEntityFrameworkStores<AppDbContext>();


var app = builder.Build();

app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.MapControllerRoute(name: "areas",pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
