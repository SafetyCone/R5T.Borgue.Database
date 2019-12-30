using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GeoAPI.Geometries;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;

using R5T.Corcyra;
using R5T.Venetia;


namespace R5T.Borgue.Database
{
    public class DatabaseCatchmentsRepository<TDbContext> : ProvidedDatabaseRepositoryBase<TDbContext>, ICatchmentsRepository
        where TDbContext: DbContext, ICatchmentsDbContext
    {
        public DatabaseCatchmentsRepository(DbContextOptions<TDbContext> dbContextOptions, IDbContextProvider<TDbContext> dbContextProvider)
            : base(dbContextOptions, dbContextProvider)
        {
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
            var catchment = this.ExecuteInContext(dbContext =>
            {
                var catchmentEntity = dbContext.GetCatchment(identity).Single();

                var output = catchmentEntity.ToAppType();
                return output;
            });

            return catchment;
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

        public async Task SetName(CatchmentIdentity identity, string name)
        {
            //using (var dbContext = this.GetNewDbContext())
            //{
            //    var entity = await dbContext.GetCatchment(identity).SingleAsync();

            //    entity.Name = name;

            //    await dbContext.SaveChangesAsync();
            //}

            await this.ExecuteInContextAsync(async dbContext =>
            {
                var entity = await dbContext.GetCatchment(identity).SingleAsync();

                entity.Name = name;

                await dbContext.SaveChangesAsync();
            });
        }

        public async Task<string> GetName(CatchmentIdentity identity)
        {
            var name = await this.ExecuteInContext(async dbContext =>
            {
                var output = await dbContext.GetCatchment(identity).Select(x => x.Name).SingleAsync();
                return output;
            });

            return name;
        }

        public async Task SetBoundary(CatchmentIdentity identity, IEnumerable<LngLat> boundaryVertices)
        {
            var polygon = boundaryVertices.ToPolygon();

            await this.ExecuteInContextAsync(async dbContext =>
            {
                var entity = await dbContext.GetCatchment(identity).SingleAsync();

                entity.Boundary = polygon;

                await dbContext.SaveChangesAsync();
            });
        }

        public async Task<IEnumerable<LngLat>> GetBoundary(CatchmentIdentity identity)
        {
            var lngLats = await this.ExecuteInContext(async dbContext =>
            {
                var entity = await dbContext.GetCatchment(identity).SingleAsync();

                var output = entity.Boundary.ToLngLats();
                return output;
            });

            return lngLats;
        }
    }
}
