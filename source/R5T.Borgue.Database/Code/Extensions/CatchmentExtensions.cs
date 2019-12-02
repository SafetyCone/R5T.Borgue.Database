using System;

using GeoAPI.Geometries;
using NetTopologySuite;
using NetTopologySuite.Geometries;

using R5T.Corcyra;

using AppType = R5T.Corcyra.Catchment;
using EntityType = R5T.Borgue.Database.Entities.Catchment;


namespace R5T.Borgue.Database
{
    public static class CatchmentExtensions
    {
        public static AppType ToAppType(this EntityType entityType)
        {
            var appType = new AppType()
            {
                Identity =  CatchmentIdentity.From(entityType.Identity),
                Name = entityType.Name,
            };

            foreach (var coordinate in entityType.Boundary.Coordinates)
            {
                appType.Boundary.Add(new LngLat() { Lng = coordinate.X, Lat = coordinate.Y }); // Is this right? X = Longitude, Y = Latitude. This is recommended by: https://docs.microsoft.com/en-us/ef/core/modeling/spatial
            }

            return appType;
        }

        public static EntityType ToEntityType(this AppType appType)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326); // Recommended SRID, used by Google Maps.

            int numCoordinates = appType.Boundary.Count;
            var coordinates = new Coordinate[numCoordinates + 1];
            for (int iCoordinate = 0; iCoordinate < numCoordinates; iCoordinate++)
            {
                var vertex = appType.Boundary[iCoordinate];

                var coordinate = new Coordinate(vertex.Lng, vertex.Lat);

                coordinates[iCoordinate] = coordinate;
            }

            // Make sure ring is closed (first point equals last point).
            var firstVertex = appType.Boundary[0];

            coordinates[numCoordinates] = new Coordinate(firstVertex.Lng, firstVertex.Lat);

            var linearRing = new LinearRing(coordinates);

            var geometry = new Polygon(linearRing, geometryFactory);

            var entity = new EntityType()
            {
                Identity = appType.Identity.Value,
                Name = appType.Name,
                Boundary = geometry,
            };

            return entity;
        }
    }
}
