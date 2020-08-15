using System;
using System.Linq;

using GeoAPI.Geometries;

using R5T.Corcyra;

using CatchmentEntity = R5T.Borgue.Database.Entities.Catchment;


namespace R5T.Borgue.Database
{
    public static class ICatchmentsDbContextExtensions
    {
        public static IQueryable<CatchmentEntity> GetCatchment(this ICatchmentsDbContext catchmentsDbContext, CatchmentIdentity catchmentIdentity)
        {
            var queryable = catchmentsDbContext.Catchments
                .Where(x => x.Identity == catchmentIdentity.Value);

            return queryable;
        }

        public static IQueryable<CatchmentEntity> GetCatchmentsContainingPoint(this ICatchmentsDbContext catchmentsDbContext, LngLat lngLat, IGeometryFactory geometryFactory)
        {
            var coordinate = new Coordinate(lngLat.Lng, lngLat.Lat);
            var point = geometryFactory.CreatePoint(coordinate);

            var queryable = catchmentsDbContext.Catchments
                .Where(x => x.Boundary.Contains(point)); // TODO: Verify no client side execution.

            return queryable;
        }

        public static IQueryable<CatchmentEntity> GetCatchmentsWithStringInNameAndWithinRadius(this ICatchmentsDbContext catchmentsDbContext, string nameContains, double radiusDegrees, LngLat lngLat, IGeometryFactory geometryFactory)
        {

            var centerCoordinate = new Coordinate(lngLat.Lng, lngLat.Lat);
            var center = geometryFactory.CreatePoint(centerCoordinate);
            var searchArea = center.Buffer(radiusDegrees).Boundary;

            var queryable = catchmentsDbContext.Catchments
                .Where(x => x.Name.Contains(nameContains))
                .Where(x => x.Boundary.Intersects(searchArea));

            return queryable;
        }

        public static IQueryable<CatchmentEntity> GetCatchmentsIntersectingRadiusFromPoint(this ICatchmentsDbContext catchmentsDbContext, double radiusDegrees, LngLat lngLat, IGeometryFactory geometryFactory)
        {
            var centerCoordinate = new Coordinate(lngLat.Lng, lngLat.Lat);
            var center = geometryFactory.CreatePoint(centerCoordinate);
            var searchArea = center.Buffer(radiusDegrees).Reverse();
            var searchPerimeter = searchArea.Boundary;

            var queryable = catchmentsDbContext.Catchments
                .Where(x => !x.Boundary.Disjoint(searchArea));
                //.Where(x => x.Boundary.Intersects(searchPerimeter));

            return queryable;
        }

        public static IQueryable<CatchmentEntity> GetCatchmentsWithStringInName(this ICatchmentsDbContext catchmentsDbContext, string nameContains)
        {
            var queryable = catchmentsDbContext.Catchments
                .Where(x => x.Name.Contains(nameContains));

            return queryable;
        }
    }
}