using Microsoft.AspNetCore.Identity; //dodajemo identity
using Microsoft.EntityFrameworkCore;
using PametniParkingSistem.Data;
using PametniParkingSistem.Models;
using PametniParkingSistem.Repositories;
using PametniParkingSistem.Services;
using PametniParkingSistem.Services.Interfaces;
using PametniParkingSistem.Settings;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

var password = builder.Configuration["DbPassword"];

if (!string.IsNullOrWhiteSpace(password))
{
    connectionString += $"Password={password};";
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// AddIdentity koristi Identity sistem sa mojom klasom Korisnik i rolama.

builder.Services.AddIdentity<Korisnik, IdentityRole>(options =>
{
    // ne traži email potvrdu prije logina
    options.SignIn.RequireConfirmedAccount = false;
})
.AddErrorDescriber<BosanskiIdentityErrorDescriber>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// repositories
builder.Services.AddScoped<IRezervacijaRepository, RezervacijaRepository>();
builder.Services.AddScoped<IParkingMjestoRepository, ParkingMjestoRepository>();
builder.Services.AddScoped<IPlacanjeRepository, PlacanjeRepository>();
builder.Services.AddScoped<IRecenzijaRepository, RecenzijaRepository>();
builder.Services.AddScoped<IKorisnikRepository, KorisnikRepository>();
builder.Services.AddScoped<ICjenovnikRepository, CjenovnikRepository>();
builder.Services.AddScoped<IEmailObavijestRepository, EmailObavijestRepository>();
builder.Services.AddScoped<IParkingZonaRepository, ParkingZonaRepository>();
builder.Services.AddScoped<IPodrskaRepository, PodrskaRepository>();

// services
builder.Services.AddScoped<IRezervacijaService, RezervacijaService>();
builder.Services.AddScoped<IParkingMjestoService, ParkingMjestoService>();
builder.Services.AddScoped<IRecenzijaService, RecenzijaService>();
builder.Services.AddScoped<IPlacanjeService, PlacanjeService>();
builder.Services.AddScoped<IParkingZonaService, ParkingZonaService>();
builder.Services.AddScoped<IKorisnikService, KorisnikService>();
builder.Services.AddScoped<IEmailObavijestService, EmailObavijestService>();
builder.Services.AddScoped<ICjenovnikService, CjenovnikService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPodrskaService, PodrskaService>();
builder.Services.AddScoped<IHomeService, HomeService>();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await DbInitializer.SeedRolesAndAdminAsync(services);
}

app.Run();