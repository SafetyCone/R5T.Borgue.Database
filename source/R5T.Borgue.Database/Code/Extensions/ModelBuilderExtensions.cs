using System;

using Microsoft.EntityFrameworkCore;


namespace R5T.Borgue.Database
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder ForCatchmentsDbContext(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Catchment>().HasAlternateKey(x => x.Identity);

            return modelBuilder;
        }

        public static ModelBuilder ForGriddedCatchmentsDbContext(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.GridUnit>()
                .HasAlternateKey(x => x.GUID)
                ;
            modelBuilder.Entity<Entities.CatchmentGridUnit>()
                .HasAlternateKey(x => new { x.CatchmentIdentity, x.GridUnitIdentity});

            return modelBuilder.ForCatchmentsDbContext();
        }
    }
}
