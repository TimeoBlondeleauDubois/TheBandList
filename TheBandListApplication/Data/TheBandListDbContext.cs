using Microsoft.EntityFrameworkCore;
using System;
using DotNetEnv;
using System.Diagnostics;
using System.IO;

namespace TheBandListApplication.Data
{
    internal class TheBandListDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Env.Load(@"..\..\..\..\TheBandListApplication\.env");
            //pour Update-Database
            // Env.Load();
            string host = Environment.GetEnvironmentVariable("DB_HOST");
            string port = Environment.GetEnvironmentVariable("DB_PORT");
            string database = Environment.GetEnvironmentVariable("DB_NAME");
            string username = Environment.GetEnvironmentVariable("DB_USERNAME");
            string password = Environment.GetEnvironmentVariable("DB_PASSWORD");


            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            string connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SslMode=Require;Trust Server Certificate=true;";
            optionsBuilder.UseNpgsql(connectionString);
        }

        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Niveau> Niveaux { get; set; }
        public DbSet<NiveauDifficulteRate> NiveauxDifficulteRates { get; set; }
        public DbSet<DifficulteFeature> DifficulteFeatures { get; set; }
        public DbSet<Pack> Packs { get; set; }
        public DbSet<PackNiveau> PackNiveaux { get; set; }
        public DbSet<ReussitePack> ReussitesPack { get; set; }
        public DbSet<Classement> Classements { get; set; }
        public DbSet<CreateurNiveau> CreateursNiveaux { get; set; }
        public DbSet<ReussiteNiveau> ReussitesNiveaux { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PackNiveau>()
                .HasKey(pn => new { pn.PackId, pn.NiveauId });

            modelBuilder.Entity<ReussitePack>()
                .HasKey(rp => new { rp.UtilisateurId, rp.PackId });

            modelBuilder.Entity<CreateurNiveau>()
                .HasKey(cn => new { cn.CreateurId, cn.NiveauId });

            modelBuilder.Entity<ReussiteNiveau>()
                .HasKey(rn => new { rn.UtilisateurId, rn.NiveauId });

            base.OnModelCreating(modelBuilder);
        }
    }
}