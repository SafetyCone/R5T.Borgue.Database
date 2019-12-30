using System;
using System.Collections.Generic;
using System.Linq;

using NetTopologySuite.Geometries;

using R5T.Corcyra;
using R5T.Magyar.Extensions;


namespace R5T.Borgue.Database
{
    public static class GeometryExtensions
    {
        /// <summary>
        /// Converts a geometry to an <see cref="LngLat"/> enumerable assuming that X-values are longitudes (and Y-values are latitudes).
        /// </summary>
        public static IEnumerable<LngLat> ToLngLatsXIsLongitude(this Geometry geometry)
        {
            var lngLats = geometry.Boundary.Coordinates.ExceptLast() // Don't include the last since it is only there to ensure the polygon ring is closed as required by NetTopologySuite.
                .Select(coordinate => new LngLat() { Lng = coordinate.X, Lat = coordinate.Y });

            return lngLats;
        }

        public static IEnumerable<LngLat> ToLngLats(this Geometry geometry)
        {
            var lngLats = geometry.ToLngLatsXIsLongitude();
            return lngLats;
        }
    }
}
