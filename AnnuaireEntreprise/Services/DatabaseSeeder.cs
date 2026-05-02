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
            // 1. On vérifie si la base est déjà remplie. Si oui, on s'arrête là pour ne pas créer de doublons.
            if (db.Salaries.Any()) return;

            // 2. On crée les services de base exigés par le sujet
            var services = new List<Service>
            {
                new Service { Nom = "Comptabilité" },
                new Service { Nom = "Production" },
                new Service { Nom = "Accueil" },
                new Service { Nom = "Informatique" }
            };
            db.Services.AddRange(services);
            await db.SaveChangesAsync();

            // 3. On appelle l'API distante avec HttpClient
            using HttpClient client = new HttpClient();
            string json = await client.GetStringAsync("https://randomuser.me/api/?results=10&nat=fr");

            // 4. On décode (parse) le JSON reçu
            using JsonDocument doc = JsonDocument.Parse(json);
            var results = doc.RootElement.GetProperty("results");

            var random = new Random();

            // 5. On boucle sur les 10 faux utilisateurs générés
            foreach (var user in results.EnumerateArray())
            {
                string nom = user.GetProperty("name").GetProperty("last").GetString();
                string prenom = user.GetProperty("name").GetProperty("first").GetString();
                string email = user.GetProperty("email").GetString();
                string telFixe = user.GetProperty("phone").GetString();
                string telPortable = user.GetProperty("cell").GetString();
                string ville = user.GetProperty("location").GetProperty("city").GetString();

                // On cherche si le Site (la ville) existe déjà dans la base, sinon on le crée
                var site = db.Sites.FirstOrDefault(s => s.Ville == ville);
                if (site == null)
                {
                    site = new Site { Ville = ville };
                    db.Sites.Add(site);
                    await db.SaveChangesAsync(); // On sauvegarde immédiatement pour générer son ID
                }

                // On lui attribue un service au hasard
                var serviceAleatoire = services[random.Next(services.Count)];

                // On assemble le Salarié avec ses clés étrangères
                var salarie = new Salarie
                {
                    Nom = nom,
                    Prenom = prenom,
                    Email = email,
                    TelephoneFixe = telFixe,
                    TelephonePortable = telPortable,
                    SiteId = site.Id,
                    ServiceId = serviceAleatoire.Id
                };

                db.Salaries.Add(salarie);
            }

            // On valide toutes les insertions en base de données
            await db.SaveChangesAsync();
        }
    }
}