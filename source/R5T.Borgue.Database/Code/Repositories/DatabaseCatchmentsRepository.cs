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

        public async Task Add(Catchment geography)
        {
            var geographyEntity = geography.ToEntityType();

            await this.ExecuteInContextAsync(async dbContext =>
            {
                dbContext.Catchments.Add(geographyEntity);

                await dbContext.SaveChangesAsync();
            });
        }

        public async Task Delete(CatchmentIdentity identity)
        {
            await this.ExecuteInContextAsync(async dbContext =>
            {
                var catchmentEntity = await dbContext.GetCatchment(identity).SingleAsync();

                dbContext.Remove(catchmentEntity);

                await dbContext.SaveChangesAsync();
            });
        }

        public Task<bool> Exists(CatchmentIdentity identity)
        {
            throw new NotImplementedException();
        }

        public async Task<Catchment> Get(CatchmentIdentity identity)
        {
            var catchment = await this.ExecuteInContextAsync(async dbContext =>
            {
                var catchmentEntity = await dbContext.GetCatchment(identity).SingleAsync();

                var output = catchmentEntity.ToAppType();
                return output;
            });

            return catchment;
        }

        public async Task<IEnumerable<Catchment>> GetAll()
        {
            var catchments = await this.ExecuteInContextAsync(async dbContext =>
            {
                var catchmentEntities = await dbContext.Catchments.ToListAsync(); // Perform query now.

                var output = catchmentEntities.Select(x => x.ToAppType());
                return output;
            });

            return catchments;
        }

        public async Task<IEnumerable<Catchment>> GetAllContainingPoint(LngLat lngLat)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            var coordinate = new Coordinate(lngLat.Lng, lngLat.Lat);
            var point = geometryFactory.CreatePoint(coordinate);

            var geographies = await this.ExecuteInContextAsync(async dbContext =>
            {
                var output = await dbContext.Catchments.Where(x => x.Boundary.Contains(point)).Select(x => x.ToAppType()).ToListAsync();
                return output;
            });

            return geographies;
        }

        public async Task SetName(CatchmentIdentity identity, string name)
        {
            await this.ExecuteInContextAsync(async dbContext =>
            {
                var entity = await dbContext.GetCatchment(identity).SingleAsync();

                entity.Name = name;

                await dbContext.SaveChangesAsync();
            });
        }

        public async Task<string> GetName(CatchmentIdentity identity)
        {
            var name = await this.ExecuteInContextAsync(async dbContext =>
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
            var lngLats = await this.ExecuteInContextAsync(async dbContext =>
            {
                var entity = await dbContext.GetCatchment(identity).SingleAsync();

                var output = entity.Boundary.ToLngLats();
                return output;
            });

            return lngLats;
        }
    }
}
