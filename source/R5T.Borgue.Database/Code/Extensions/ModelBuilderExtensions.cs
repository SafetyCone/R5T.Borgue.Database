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
    }
}
