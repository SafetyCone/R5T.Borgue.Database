using System;
using System.Linq;

using GridUnitEntity = R5T.Borgue.Database.Entities.GridUnit;
namespace R5T.Borgue.Database
{
    public static class GridUnitsQueryableExtensions
    {
        public static IQueryable<GridUnitEntity> GetByIdentity(this IQueryable<GridUnitEntity> gridUnits, GridUnitIdentity gridUnitIdentity)
        {
            var queryable = gridUnits.Where(x => x.GUID == gridUnitIdentity.Value);
            return queryable;
        }
    }
}
