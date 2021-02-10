using System;
using System.Collections.Generic;
using System.Linq;

using NetTopologySuite.Geometries;

using R5T.Corcyra;
using R5T.Magyar;
using R5T.Magyar.Extensions;


namespace R5T.Borgue.Database
{
    public static class GeometryExtensions
    {
        /// <summary>
        /// Converts a geometry to an <see cref="LngLat"/> enumerable assuming that X-values are longitudes (and Y-values are latitudes).
        /// </summary>
        public static LngLat[] ToLngLatsXIsLongitude(this Geometry geometry)
        {
            var lngLats = geometry.Boundary.Coordinates.ExceptLast()
                .Select(coordinate => new LngLat() { Lng = coordinate.X, Lat = coordinate.Y })
                .ToArray();

            return lngLats;
        }

        public static LngLat[] ToLngLats(this Geometry geometry)
        {
            var lngLats = geometry.ToLngLatsXIsLongitude();
            return lngLats;
        }
    }
}
