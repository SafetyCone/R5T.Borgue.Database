using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using GeoAPI.Geometries;

using R5T.Corcyra;
using R5T.Venetia;

using CatchmentEntity = R5T.Borgue.Database.Entities.Catchment;


namespace R5T.Borgue.Database
{
    public class DatabaseCatchmentsRepository<TDbContext> : ProvidedDatabaseRepositoryBase<TDbContext>, ICatchmentsRepository
        where TDbContext: DbContext, ICatchmentsDbContext, IGridUnitsDbContext
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

        // Methods for getting catchments for a point (e.g. for anomaly catchment assignment or goastbusters)
        private async Task<List<CatchmentEntity>> GetCatchmentEntitiesContainingPoint(LngLat lngLat)
        {
            var geometryFactory = await this.GeometryFactoryProvider.GetGeometryFactoryAsync();
            var catchmentEntities = await this.ExecuteInContextAsync(async dbContext =>
            {
                var coordinate = new Coordinate(lngLat.Lng, lngLat.Lat);
                var point = geometryFactory.CreatePoint(coordinate);
                var relevantGridUnitIdentity = await dbContext.GridUnits
                    .Where(x => x.Boundary.Contains(point))
                    .Select(x => GridUnitIdentity.From(x.GUID))
                    .SingleOrDefaultAsync();

                IQueryable<CatchmentEntity> result;
                if (relevantGridUnitIdentity == default)
                {
                    // Search all catchments. This may take a while.
                    result = dbContext.Catchments
                        .Where(x => x.Boundary.Contains(point));
                }
                else
                {
                    result = from catchment in dbContext.Catchments
                             join catchmentGridUnit  in dbContext.CatchmentGridUnits
                             on catchment.Identity equals catchmentGridUnit.CatchmentIdentity
                             where catchmentGridUnit.GridUnitIdentity == relevantGridUnitIdentity.Value
                             where catchment.Boundary.Contains(point)
                             select catchment;
                }
                return await result.ToListAsync();
            });

            return catchmentEntities;
        }

        public async Task<List<Catchment>> GetAllContainingPoint(LngLat lngLat)
        {
            var catchmentEntities = await this.GetCatchmentEntitiesContainingPoint(lngLat);
            var catchmentList = catchmentEntities
                .Select(x => x.ToAppType())
                .ToList();
            return catchmentList;
        }

        public async Task<List<CatchmentIdentity>> GetAllCatchmentIdentitiesContainingPointAsync(LngLat lngLat)
        {
            var catchmentEntities = await this.GetCatchmentEntitiesContainingPoint(lngLat);
            var catchmentIdentityList = catchmentEntities
                .Select(x => CatchmentIdentity.From(x.Identity))
                .ToList();
            return catchmentIdentityList;
        }

        // Filter catchments by location and/or name
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

        // Getters and setters for catchment components
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
    }
}
