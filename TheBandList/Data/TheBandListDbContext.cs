using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TheBandListApplication.Data;

namespace TheBandList.Data
{
    public class TheBandListDbContext : DbContext
    {
        public TheBandListDbContext(DbContextOptions<TheBandListDbContext> options)
        : base(options)
        { }

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
        public DbSet<SoumissionNiveau> SoumissionsNiveaux { get; set; }

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
