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
    public class DatabaseGridUnitsRepository<TDbContext> : ProvidedDatabaseRepositoryBase<TDbContext>, IGridUnitsRepository
        where TDbContext: DbContext, ICatchmentsDbContext, IGridUnitsDbContext
    {
        private IGeometryFactoryProvider GeometryFactoryProvider { get; }


        public DatabaseGridUnitsRepository(DbContextOptions<TDbContext> dbContextOptions, IDbContextProvider<TDbContext> dbContextProvider,
            IGeometryFactoryProvider geometryFactoryProvider)
            : base(dbContextOptions, dbContextProvider)
        {
            this.GeometryFactoryProvider = geometryFactoryProvider;
        }

        public async Task SetGridUnitsForCatchment(CatchmentIdentity catchmentIdentity)
        {
            await this.ExecuteInContextAsync(async dbContext =>
            {
                // Get the right catchment (also verifies that it's in the database)
                var catchmentEntity = await dbContext.Catchments
                    .Where(x => x.Identity == catchmentIdentity.Value)
                    .SingleAsync();

                // Delete existing grid unit affiliations (but NOT the grid units!!)
                var matchingGridUnitAffiliations = await dbContext.CatchmentGridUnits
                    .Where(x => x.CatchmentIdentity == catchmentIdentity.Value)
                    .ToListAsync();
                if (matchingGridUnitAffiliations.Count > 0)
                {
                    dbContext.RemoveRange(matchingGridUnitAffiliations);
                    // TODO: do we need this save changes before adding the new entities?
                    await dbContext.SaveChangesAsync();
                }

                // Find its intersecting grid units
                var gridUnitIdentities= await dbContext.GridUnits
                    .Where(x => x.Boundary.Intersects(catchmentEntity.Boundary))
                    .Select(x => GridUnitIdentity.From(x.GUID))
                    .ToListAsync();

                // Add them to the database
                foreach (var gridUnitIdentity in gridUnitIdentities)
                {
                    // TODO: update to addrange once I have time to check that it works
                    // (which should speed this sucker up a bit.)
                    var newCatchmentGridUnitEntity = new Entities.CatchmentGridUnit
                    {
                        GridUnitIdentity = gridUnitIdentity.Value,
                        CatchmentIdentity = catchmentIdentity.Value
                    };
                    dbContext.Add(newCatchmentGridUnitEntity);
                }

                await dbContext.SaveChangesAsync();
            });
        }

        public Task Add(GridUnit geography)
        {
            throw new NotImplementedException();
        }

        public Task Delete(GridUnitIdentity identity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(GridUnitIdentity identity)
        {
            throw new NotImplementedException();
        }

        public Task<GridUnit> Get(GridUnitIdentity identity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GridUnit>> GetAll()
        {
            throw new NotImplementedException();
        }

        // Getters and setters for catchment components
        public async Task SetName(GridUnitIdentity identity, string name)
        {
            await this.ExecuteInContextAsync(async dbContext =>
            {
                var entity = await dbContext.GridUnits.GetByIdentity(identity).SingleAsync();

                entity.Name = name;

                await dbContext.SaveChangesAsync();
            });
        }

        public Task<string> GetName(GridUnitIdentity identity)
        {
            return this.ExecuteInContextAsync(async dbContext =>
            {
                var name = await dbContext.GridUnits.GetByIdentity(identity).Select(x => x.Name).SingleAsync();
                return name;
            });
        }

        public async Task SetBoundary(GridUnitIdentity identity, IEnumerable<LngLat> boundaryVertices)
        {
            var geometryFactory = await this.GeometryFactoryProvider.GetGeometryFactoryAsync();

            var polygon = boundaryVertices.ToPolygon(geometryFactory);

            await this.ExecuteInContextAsync(async dbContext =>
            {
                var entity = await dbContext.GridUnits.GetByIdentity(identity).SingleAsync();

                entity.Boundary = polygon;

                await dbContext.SaveChangesAsync();
            });
        }

        public Task<LngLat[]> GetBoundary(GridUnitIdentity identity)
        {
            return this.ExecuteInContextAsync(async dbContext =>
            {
                var entity = await dbContext.GridUnits.GetByIdentity(identity).SingleAsync();

                var lngLats = entity.Boundary.ToLngLats();
                return lngLats;
            });
        }
    }
}
