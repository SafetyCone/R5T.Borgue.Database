using System;

using Microsoft.EntityFrameworkCore;


namespace R5T.Borgue.Database
{
    public class GeographyDbContext : DbContext
    {
        public DbSet<Entities.Geography> Geographies { get; set; }


        public GeographyDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Entities.Geography>().HasAlternateKey(x => x.Identity);
        }
    }
}
