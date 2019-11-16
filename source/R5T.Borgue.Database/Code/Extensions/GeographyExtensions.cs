using System;

using GeoAPI.Geometries;
using NetTopologySuite;
using NetTopologySuite.Geometries;

using R5T.Corcyra;

using AppType = R5T.Corcyra.Geography;
using EntityType = R5T.Borgue.Database.Entities.Geography;


namespace R5T.Borgue.Database
{
    public static class GeographyExtensions
    {
        public static AppType ToAppType(this EntityType entityType)
        {
            var appType = new AppType()
            {
                Identity =  GeographyIdentity.From(entityType.Identity)
            };

            foreach (var coordinate in entityType.Border.Coordinates)
            {
                appType.Vertices.Add(new LngLat() { Lng = coordinate.X, Lat = coordinate.Y }); // Is this right? X = Longitude, Y = Latitude. This is recommended by: https://docs.microsoft.com/en-us/ef/core/modeling/spatial
            }

            return appType;
        }

        public static EntityType ToEntityType(this AppType appType)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326); // Recommended SRID, used by Google Maps.

            int numCoordinates = appType.Vertices.Count;
            var coordinates = new Coordinate[numCoordinates];
            for (int iCoordinate = 0; iCoordinate < numCoordinates; iCoordinate++)
            {
                var vertex = appType.Vertices[iCoordinate];

                var coordinate = new Coordinate(vertex.Lng, vertex.Lat);

                coordinates[iCoordinate] = coordinate;
            }

            var linearRing = new LinearRing(coordinates);

            var geometry = new Polygon(linearRing);

            var entity = new EntityType()
            {
                Identity = appType.Identity.Value,
                Border = geometry,
            };

            return entity;
        }
    }
}
