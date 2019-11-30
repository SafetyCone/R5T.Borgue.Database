using System;

using Microsoft.EntityFrameworkCore;


namespace R5T.Borgue.Database
{
    public class CatchmentsDbContext : DbContext
    {
        public DbSet<Entities.Catchment> Catchments { get; set; }


        public CatchmentsDbContext(DbContextOptions<CatchmentsDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Entities.Catchment>().HasAlternateKey(x => x.Identity);
        }
    }
}
