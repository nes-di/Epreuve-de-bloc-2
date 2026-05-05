using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AnnuaireEntreprise.Models;
using AnnuaireEntreprise.Data;

namespace AnnuaireEntreprise.Services
{
    public static class DatabaseSeeder
    {
        public static async Task SeedDataAsync(AppDbContext db)
        {
            if (db.Salaries.Any()) return;
            await InitialiserServices(db);
            await Import10UsersAsync(db);
        }

        public static async Task Import10UsersAsync(AppDbContext db)
        {
            await InitialiserServices(db);
            var services = db.Services.ToList();
            using HttpClient client = new HttpClient();
            string json = await client.GetStringAsync("https://randomuser.me/api/?results=10&nat=fr");

            using JsonDocument doc = JsonDocument.Parse(json);
            var results = doc.RootElement.GetProperty("results");
            var random = new Random();

            foreach (var user in results.EnumerateArray())
            {
                string nom = user.GetProperty("name").GetProperty("last").GetString()?.ToUpper() ?? "";
                string prenom = user.GetProperty("name").GetProperty("first").GetString() ?? "";
                string email = user.GetProperty("email").GetString() ?? "";
                string telFixe = user.GetProperty("phone").GetString() ?? "";
                string telPortable = user.GetProperty("cell").GetString() ?? "";
                string ville = user.GetProperty("location").GetProperty("city").GetString() ?? "";

                var site = db.Sites.FirstOrDefault(s => s.Ville == ville);
                if (site == null)
                {
                    site = new Site { Ville = ville };
                    db.Sites.Add(site);
                    await db.SaveChangesAsync(); 
                }

                var salarie = new Salarie
                {
                    Nom = nom, Prenom = prenom, Email = email,
                    TelephoneFixe = telFixe, TelephonePortable = telPortable,
                    SiteId = site.Id, ServiceId = services[random.Next(services.Count)].Id
                };
                db.Salaries.Add(salarie);
            }
            await db.SaveChangesAsync();
        }

        private static async Task InitialiserServices(AppDbContext db)
        {
            if (!db.Services.Any())
            {
                db.Services.AddRange(new List<Service>
                {
                    new Service { Nom = "Comptabilité" }, new Service { Nom = "Production" },
                    new Service { Nom = "Accueil" }, new Service { Nom = "Informatique" }
                });
                await db.SaveChangesAsync();
            }
        }
    }
}