using System;
using System.Collections.Generic;
using System.Linq;

using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

using R5T.Corcyra;


namespace R5T.Borgue.Database
{
    public static class LngLatExtensions
    {
        public static Coordinate ToCoordinateXIsLongitude(this LngLat lngLat)
        {
            var coordinate = new Coordinate(lngLat.Lng, lngLat.Lat);
            return coordinate;
        }

        /// <summary>
        /// Uses <see cref="ToCoordinateXIsLongitude(LngLat)"/> as the default.
        /// </summary>
        public static Coordinate ToCoordinate(this LngLat lngLat)
        {
            var coordinate = lngLat.ToCoordinateXIsLongitude();
            return coordinate;
        }

        /// <summary>
        /// Provides a polygon from <see cref="LngLat"/> values assuming that polygon coordinate X-values are longitudes (and Y-values are latitudes).
        /// </summary>
        public static Polygon ToPolygonXIsLongitude(this IEnumerable<LngLat> lngLats, IGeometryFactory geometryFactory)
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

            var polygon = new Polygon(linearRing, geometryFactory);
            return polygon;
        }

        public static Polygon ToPolygon(this IEnumerable<LngLat> lngLats, IGeometryFactory geometryFactory)
        {
            var polygon = lngLats.ToPolygonXIsLongitude(geometryFactory);
            return polygon;
        }
    }
}
