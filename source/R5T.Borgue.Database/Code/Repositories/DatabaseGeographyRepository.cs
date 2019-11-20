using System;
using System.Collections.Generic;
using System.Linq;

using GeoAPI.Geometries;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

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

        public IEnumerable<Geography> GetAll()
        {
            using (var dbContext = this.GetNewDbContext())
            {
                var geographyEntities = dbContext.Geographies.ToList(); // Perform query now.
                foreach (var geographyEntity in geographyEntities)
                {
                    var geography = geographyEntity.ToAppType();

                    yield return geography;
                }
            }
        }

        public IEnumerable<Geography> GetGeographiesContainingPoint(LngLat lngLat)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var coordinate = new Coordinate(lngLat.Lng, lngLat.Lat);
            var point = geometryFactory.CreatePoint(coordinate);

            using (var dbContext = this.GetNewDbContext())
            {
                var geographies = dbContext.Geographies.Where(x => x.Border.Contains(point)).Select(x => x.ToAppType()).ToList();
                return geographies;
            }
        }
    }
}
