using Microsoft.EntityFrameworkCore;
using AnnuaireEntreprise.Models;
using System.IO;

namespace AnnuaireEntreprise.Data
{
    public class AppDbContext : DbContext
    {
        // On déclare nos tables SQL (DbSet) en se basant sur nos modèles
        public DbSet<Site> Sites { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Salarie> Salaries { get; set; }

        public AppDbContext()
        {
            // Cette commande magique crée le fichier SQLite et les tables
            // automatiquement la première fois qu'on lance l'application.
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // MAUI est multi-plateforme (Windows, Android, iOS). 
            // FileSystem.AppDataDirectory trouve automatiquement le bon dossier 
            // sécurisé sur la machine de l'utilisateur pour stocker la base de données.
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "annuaire.db");
            
            // On connecte notre application au fichier SQLite
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }
    }
}