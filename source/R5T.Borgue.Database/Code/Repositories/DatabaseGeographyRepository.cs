using System;

using Microsoft.EntityFrameworkCore;

using R5T.Corcyra;
using R5T.Venetia;


namespace R5T.Borgue.Database
{
    public class DatabaseGeographyRepository : DatabaseRepositoryBase<GeographyDbContext>, IGeographyRepository
    {
        public DatabaseGeographyRepository(DbContextOptions<GeographyDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public override GeographyDbContext GetNewDbContext()
        {
            var dbContext = new GeographyDbContext(this.DbContextOptions);
            return dbContext;
        }

        public void Add(Geography geography)
        {
            var geographyEntity = geography.ToEntityType();

            using (var dbContext = this.GetNewDbContext())
            {
                dbContext.Geographies.Add(geographyEntity);

                dbContext.SaveChanges();
            }
        }

        public void Delete(GeographyIdentity identity)
        {
            throw new NotImplementedException();
        }

        public bool Exists(GeographyIdentity identity)
        {
            throw new NotImplementedException();
        }

        public Geography Get(GeographyIdentity identity)
        {
            throw new NotImplementedException();
        }
    }
}
