using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using FUNSAR.AccesoDatos.Data;
using FUNSAR.AccesoDatos.Data.Repository.IRepository;
using FUNSAR.AccesoDatos.Data.Repository;
using Microsoft.AspNetCore.Identity;
using FUNSAR.Models;
using FUNSAR.AccesoDatos.Data.Inicializador;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ConexionSQL");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IContenedorTrabajo, ContenedorTrabajo>();

builder.Services.AddScoped<IInicializadorDB, InicializadorDB>();

builder.Services.AddHttpClient();

builder.Services.AddHostedService<PaymentValidationService>();

builder.Services.AddHostedService<ProcessValidationService>();

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (!context.Request.IsHttps)
    {
        var httpsUrl = $"https://{context.Request.Host}{context.Request.Path}";
        context.Response.Redirect(httpsUrl);
        return;
    }

    await next();
});


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

SiembraDeDatos();

app.UseRouting();

app.UseSession();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
    {
        var httpsUrl = $"https://{context.Request.Host}";
        context.Response.Redirect(httpsUrl);
        return;
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Cliente}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

IWebHostEnvironment env = app.Environment;
Rotativa.AspNetCore.RotativaConfiguration.Setup(env.WebRootPath, "../wwwroot/Rotativa/Windows/");


app.Run();

//Funcionalidad metodo SiembraDeDatos()

void SiembraDeDatos()
{
    using (var scope = app.Services.CreateScope())
    {
        var inicializadorDb = scope.ServiceProvider.GetRequiredService<IInicializadorDB>();
        inicializadorDb.Inicializar();
    }
}