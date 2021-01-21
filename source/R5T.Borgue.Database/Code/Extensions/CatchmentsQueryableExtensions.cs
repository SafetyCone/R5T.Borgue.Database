using System;
using System.Linq;

using GeoAPI.Geometries;

using R5T.Corcyra;

using CatchmentEntity = R5T.Borgue.Database.Entities.Catchment;


namespace R5T.Borgue.Database
{
    public static class CatchmentsQueryableExtensions
    {
        public static IQueryable<CatchmentEntity> GetWithinRadius(this IQueryable<CatchmentEntity> catchments, double radiusInDegrees, LngLat lngLat, IGeometryFactory geometryFactory)
        {
            var centerCoordinate = new Coordinate(lngLat.Lng, lngLat.Lat);
            var centerPoint = geometryFactory.CreatePoint(centerCoordinate);
            var searchArea = centerPoint.Buffer(radiusInDegrees).Reverse();

            var queryable = catchments.Where(x => !x.Boundary.Disjoint(searchArea));
            return queryable;
        }

        public static IQueryable<CatchmentEntity> GetContainingName(this IQueryable<CatchmentEntity> catchments, string nameContains)
        {
            var queryable = catchments.Where(x => x.Name.Contains(nameContains));
            return queryable;
        }

        public static IQueryable<CatchmentEntity> GetByIdentity(this IQueryable<CatchmentEntity> catchments, CatchmentIdentity catchmentIdentity)
        {
            var queryable = catchments.Where(x => x.Identity == catchmentIdentity.Value);
            return queryable;
        }

        // Removed because with gridding, this can't be an extension for a single table,
        // it needs three tables (Catchments, GridUnits, and CatchmentGridUnits) to work
        // public static IQueryable<CatchmentEntity> GetCatchmentsContainingPoint(this IQueryable<CatchmentEntity> catchments, LngLat lngLat, IGeometryFactory geometryFactory)
        // {
        //     var coordinate = new Coordinate(lngLat.Lng, lngLat.Lat);
        //     var point = geometryFactory.CreatePoint(coordinate);

        //     var queryable = catchments.Where(x => x.Boundary.Contains(point));
        //     return queryable;
        // }

        public static IQueryable<CatchmentEntity> GetCatchmentsWithStringInNameAndWithinRadius(this IQueryable<CatchmentEntity> catchments, string nameContains, double radiusDegrees, LngLat lngLat, IGeometryFactory geometryFactory)
        {
            var centerCoordinate = new Coordinate(lngLat.Lng, lngLat.Lat);
            var center = geometryFactory.CreatePoint(centerCoordinate);
            var searchArea = center.Buffer(radiusDegrees).Boundary;

            var queryable = catchments
                .GetContainingName(nameContains)
                .GetWithinRadius(radiusDegrees, lngLat, geometryFactory)
                ;

            return queryable;
        }
    }
}
