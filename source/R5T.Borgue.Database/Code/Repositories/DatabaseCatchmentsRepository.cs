using System;
using System.Collections.Generic;
using System.Linq;

using GeoAPI.Geometries;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;

using R5T.Corcyra;
using R5T.Venetia;


namespace R5T.Borgue.Database
{
    public class DatabaseCatchmentsRepository : DatabaseRepositoryBase<CatchmentsDbContext>, ICatchmentsRepository
    {
        public DatabaseCatchmentsRepository(DbContextOptions<CatchmentsDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }

        public override CatchmentsDbContext GetNewDbContext()
        {
            var dbContext = new CatchmentsDbContext(this.DbContextOptions);
            return dbContext;
        }

        public void Add(Catchment geography)
        {
            var geographyEntity = geography.ToEntityType();

            using (var dbContext = this.GetNewDbContext())
            {
                dbContext.Catchments.Add(geographyEntity);

                dbContext.SaveChanges();
            }
        }

        public void Delete(CatchmentIdentity identity)
        {
            throw new NotImplementedException();
        }

        public bool Exists(CatchmentIdentity identity)
        {
            throw new NotImplementedException();
        }

        public Catchment Get(CatchmentIdentity identity)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Catchment> GetAll()
        {
            using (var dbContext = this.GetNewDbContext())
            {
                var geographyEntities = dbContext.Catchments.ToList(); // Perform query now.
                foreach (var geographyEntity in geographyEntities)
                {
                    var geography = geographyEntity.ToAppType();

                    yield return geography;
                }
            }
        }

        public IEnumerable<Catchment> GetAllContainingPoint(LngLat lngLat)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var coordinate = new Coordinate(lngLat.Lng, lngLat.Lat);
            var point = geometryFactory.CreatePoint(coordinate);

            using (var dbContext = this.GetNewDbContext())
            {
                var geographies = dbContext.Catchments.Where(x => x.Boundary.Contains(point)).Select(x => x.ToAppType()).ToList();
                return geographies;
            }
        }

        public void SetName(CatchmentIdentity identity, string name)
        {
            throw new NotImplementedException();
        }

        public string GetName(CatchmentIdentity identity)
        {
            throw new NotImplementedException();
        }
    }
}
