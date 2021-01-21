using System;
using System.Linq;

using GeoAPI.Geometries;

using R5T.Corcyra;

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

        public static IQueryable<GridUnitEntity> GetWithinRadius(this IQueryable<GridUnitEntity> gridUnits, double radiusInDegrees, LngLat lngLat, IGeometryFactory geometryFactory)
        {
            var centerCoordinate = new Coordinate(lngLat.Lng, lngLat.Lat);
            var centerPoint = geometryFactory.CreatePoint(centerCoordinate);
            var searchArea = centerPoint.Buffer(radiusInDegrees).Reverse();

            var queryable = gridUnits.Where(x => !x.Boundary.Disjoint(searchArea));
            return queryable;
        }
    }
}
