using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using GeoAPI.Geometries;

using R5T.Corcyra;
using R5T.Venetia;


namespace R5T.Borgue.Database
{
    public class DatabaseCatchmentsRepository<TDbContext> : ProvidedDatabaseRepositoryBase<TDbContext>, ICatchmentsRepository
        where TDbContext: DbContext, ICatchmentsDbContext
    {
        private IGeometryFactoryProvider GeometryFactoryProvider { get; }


        public DatabaseCatchmentsRepository(DbContextOptions<TDbContext> dbContextOptions, IDbContextProvider<TDbContext> dbContextProvider,
            IGeometryFactoryProvider geometryFactoryProvider)
            : base(dbContextOptions, dbContextProvider)
        {
            this.GeometryFactoryProvider = geometryFactoryProvider;
        }

        public async Task Add(Catchment geography)
        {
            var geographyFactory = await this.GeometryFactoryProvider.GetGeometryFactoryAsync();

            var geographyEntity = geography.ToEntityType(geographyFactory);

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
                var catchmentEntity = await dbContext.GetCatchment(identity).SingleOrDefaultAsync();

                if (catchmentEntity == null)
                {
                    return null;
                }

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
            var geometryFactory = await this.GeometryFactoryProvider.GetGeometryFactoryAsync();

            var catchments = await this.ExecuteInContextAsync(async dbContext =>
            {
                var output = await dbContext
                    .GetCatchmentsContainingPoint(lngLat, geometryFactory)
                    .Select(x => x.ToAppType())
                    .ToListAsync(); // Execute now to avoid disposing DbContext.

                return output;
            });

            return catchments;
        }

        public async Task<List<Catchment>> GetFilteredByName(string nameContains)
        {
            var catchments = await this.ExecuteInContextAsync(async dbContext =>
            {
                var output = await dbContext
                    .GetCatchmentsWithStringInName(nameContains)
                    .Select(x => x.ToAppType())
                    .ToListAsync(); // Execute now to avoid disposing DbContext.

                return output;
            });

            return catchments;
        }

        public async Task<List<Catchment>> GetFilteredByNameAndRadius(string nameContains, double radiusDegrees, LngLat lngLat)
        {
            var geometryFactory = await this.GeometryFactoryProvider.GetGeometryFactoryAsync();

            var catchments = await this.ExecuteInContextAsync(async dbContext =>
            {
                var output = await dbContext
                    .GetCatchmentsWithStringInNameAndWithinRadius(nameContains, radiusDegrees, lngLat, geometryFactory)
                    .Select(x => x.ToAppType())
                    .ToListAsync(); // Execute now to avoid disposing DbContext.

                return output;
            });

            return catchments;
        }

        public async Task<List<Catchment>> GetAllWithinRadiusOfPoint(double radiusDegrees, LngLat lngLat)
        {
            var geometryFactory = await this.GeometryFactoryProvider.GetGeometryFactoryAsync();

            var catchments = await this.ExecuteInContextAsync(async dbContext =>
            {
                var output = await dbContext
                    .GetCatchmentsIntersectingRadiusFromPoint(radiusDegrees, lngLat, geometryFactory)
                    .Select(x => x.ToAppType())
                    .ToListAsync(); // Execute now to avoid disposing DbContext.

                return output;
            });

            return catchments;
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
            var geometryFactory = await this.GeometryFactoryProvider.GetGeometryFactoryAsync();

            var polygon = boundaryVertices.ToPolygon(geometryFactory);

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

        public async Task<List<CatchmentIdentity>> GetAllCatchmentIdentitiesContainingPointAsync(LngLat lngLat)
        {
            var geometryFactory = await this.GeometryFactoryProvider.GetGeometryFactoryAsync();

            var catchmentIdentityValues = await this.ExecuteInContextAsync(async dbContext =>
            {
                var output = await dbContext
                    .GetCatchmentsContainingPoint(lngLat, geometryFactory)
                    .Select(x => x.Identity)
                    .ToListAsync(); // Execute now to avoid disposing DbContext.

                return output;
            });

            var catchmentIdentities = catchmentIdentityValues.Select(x => CatchmentIdentity.From(x)).ToList();
            return catchmentIdentities;
        }
    }
}
