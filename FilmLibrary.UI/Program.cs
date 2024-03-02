using FilmLibrary.Business.Interfaces;
using FilmLibrary.Business.Services;
using FilmLibrary.DAL.Context;
using FilmLibrary.DAL.Interfaces;
using FilmLibrary.DAL.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var configurationBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var options = new DbContextOptionsBuilder().Options;
builder.Services.AddDbContext<ApplicationDbContext>(_ => new ApplicationDbContext(options: options, configurationBuilder));
builder.Services.AddTransient<IGenreService, GenreService>();
builder.Services.AddTransient<IArtistService, ArtistService>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IMovieRepository, MovieRepository>();
builder.Services.AddTransient<IArtistRepository, ArtistRepository>();
builder.Services.AddTransient<IGenreRepository, GenreRepository>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IMovieService, MovieService>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IAuthentificationService, AuthentificationService>();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.LoginPath = new PathString("/User/Login");
    options.AccessDeniedPath = new PathString("/Home/AccessDenied");
});
builder.Services.AddAuthorization();

var app = builder.Build();


app.UseStatusCodePages(async statusCodeContext =>
{
    var response = statusCodeContext.HttpContext.Response;
    var path = statusCodeContext.HttpContext.Request.Path;

    response.ContentType = "text/plain";
    if (response.StatusCode == 403)
    {
        await response.WriteAsync($"Path: {path}. Access Denied ");
    }
    else if (response.StatusCode == 404)
    {
        await response.WriteAsync($"Resource {path} Not Found");
    }
});


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=LaunchStartPage}/{id?}");

app.Run();