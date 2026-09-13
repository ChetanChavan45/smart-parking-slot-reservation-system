using Microsoft.AspNetCore.Identity;
using PametniParkingSistem.Enums;
using PametniParkingSistem.Models;

namespace PametniParkingSistem.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            //kreiranje rola, brisanje rola, provjera da li rola postoji
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<Korisnik>>();

            string[] roleNames =
            {
                "Administrator",
                "Operater",
                "RegistrovaniKorisnik"
            };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    //RoleManager pravi role 
                    //Role se kreiraju jednom pri pokretanju aplikacije, zato se nalazi ovdje
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            string adminEmail = "admin@parking.ba";
            string adminPassword = "Admin123!";

            var admin = await userManager.FindByEmailAsync(adminEmail);

            if (admin == null)
            {
                admin = new Korisnik
                {
                    Ime = "Admin",
                    Prezime = "Sistema",
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    DatumRegistracije = DateTime.Now,
                    StatusNaloga = StatusNaloga.Aktivan,
                    Uloga = Uloga.Administrator
                };

                var result = await userManager.CreateAsync(admin, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Administrator");
                }
            }
        }
    }
}