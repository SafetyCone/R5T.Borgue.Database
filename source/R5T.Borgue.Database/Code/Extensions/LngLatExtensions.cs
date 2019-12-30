using System;
using System.Collections.Generic;
using System.Linq;

using GeoAPI.Geometries;
using NetTopologySuite;
using NetTopologySuite.Geometries;

using R5T.Corcyra;


namespace R5T.Borgue.Database
{
    public static class LngLatExtensions
    {
        /// <summary>
        /// Provides a polygon from <see cref="LngLat"/> values assuming that polygon coordinate X-values are longitudes (and Y-values are latitudes).
        /// </summary>
        public static Polygon ToPolygonXIsLongitude(this IEnumerable<LngLat> lngLats)
        {
            var coordinates = new List<Coordinate>();
            foreach (var lngLat in lngLats)
            {
                var coordinate = new Coordinate(lngLat.Lng, lngLat.Lat);

                coordinates.Add(coordinate);
            }

            // Make sure polygon ring is closed (first coordinate is the same as the last coordinate).
            var firstCoordinate = coordinates.First();
            var lastCoordinate = coordinates.Last();

            if(firstCoordinate != lastCoordinate)
            {
                var finalCoordinate = firstCoordinate.Copy();

                coordinates.Add(finalCoordinate);
            }

            var linearRing = new LinearRing(coordinates.ToArray());

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326); // Recommended SRID, used by Google Maps.

            var polygon = new Polygon(linearRing, geometryFactory);
            return polygon;
        }

        public static Polygon ToPolygon(this IEnumerable<LngLat> lngLats)
        {
            var polygon = lngLats.ToPolygonXIsLongitude();
            return polygon;
        }
    }
}
