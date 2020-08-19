using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

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
                var catchmentEntity = await dbContext.Catchments.GetByIdentity(identity).SingleAsync();

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
                var catchmentEntity = await dbContext.Catchments.GetByIdentity(identity).SingleOrDefaultAsync();

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

        public async Task<List<Catchment>> GetAllContainingPoint(LngLat lngLat)
        {
            var geometryFactory = await this.GeometryFactoryProvider.GetGeometryFactoryAsync();

            var catchments = await this.ExecuteInContextAsync(async dbContext =>
            {
                var output = await dbContext.Catchments
                    .GetCatchmentsContainingPoint(lngLat, geometryFactory)
                    .Select(x => x.ToAppType())
                    .ToListAsync(); // Execute now to avoid disposing DbContext.

                return output;
            });

            return catchments;
        }

        public Task<List<Catchment>> GetFilteredByName(string nameContains)
        {
            return this.ExecuteInContextAsync(async dbContext =>
            {
                var catchments = await dbContext.Catchments
                    .GetContainingName(nameContains)
                    .Select(x => x.ToAppType())
                    .ToListAsync(); // Execute now to avoid disposing DbContext.

                return catchments;
            });
        }

        public async Task<List<Catchment>> GetFilteredByNameAndRadius(string nameContains, double radiusDegrees, LngLat lngLat)
        {
            var geometryFactory = await this.GeometryFactoryProvider.GetGeometryFactoryAsync();

            var catchments = await this.ExecuteInContextAsync(async dbContext =>
            {
                var output = await dbContext.Catchments
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
                var output = await dbContext.Catchments
                    .GetWithinRadius(radiusDegrees, lngLat, geometryFactory)
                    .Select(x => x.ToAppType())
                    .ToListAsync(); // Execute now to avoid disposing DbContext.

                return output;
            });

            return catchments;
        }

        public Task SetName(CatchmentIdentity identity, string name)
        {
            return this.ExecuteInContextAsync(async dbContext =>
            {
                var entity = await dbContext.Catchments.GetByIdentity(identity).SingleAsync();

                entity.Name = name;

                await dbContext.SaveChangesAsync();
            });
        }

        public Task<string> GetName(CatchmentIdentity identity)
        {
            return this.ExecuteInContextAsync(async dbContext =>
            {
                var name = await dbContext.Catchments.GetByIdentity(identity).Select(x => x.Name).SingleAsync();
                return name;
            });
        }

        public async Task SetBoundary(CatchmentIdentity identity, IEnumerable<LngLat> boundaryVertices)
        {
            var geometryFactory = await this.GeometryFactoryProvider.GetGeometryFactoryAsync();

            var polygon = boundaryVertices.ToPolygon(geometryFactory);

            await this.ExecuteInContextAsync(async dbContext =>
            {
                var entity = await dbContext.Catchments.GetByIdentity(identity).SingleAsync();

                entity.Boundary = polygon;

                await dbContext.SaveChangesAsync();
            });
        }

        public Task<IEnumerable<LngLat>> GetBoundary(CatchmentIdentity identity)
        {
            return this.ExecuteInContextAsync(async dbContext =>
            {
                var entity = await dbContext.Catchments.GetByIdentity(identity).SingleAsync();

                var lngLats = entity.Boundary.ToLngLats();
                return lngLats;
            });
        }

        public async Task<List<CatchmentIdentity>> GetAllCatchmentIdentitiesContainingPointAsync(LngLat lngLat)
        {
            var geometryFactory = await this.GeometryFactoryProvider.GetGeometryFactoryAsync();

            var catchmentIdentityValues = await this.ExecuteInContextAsync(async dbContext =>
            {
                var output = await dbContext.Catchments
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
