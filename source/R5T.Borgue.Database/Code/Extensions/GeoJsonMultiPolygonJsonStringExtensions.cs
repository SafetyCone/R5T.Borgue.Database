using System;
using System.Collections.Generic;
using System.Linq;

using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;

using R5T.Corcyra;


namespace R5T.Borgue.Database
{
    public static class GeoJsonMultiPolygonJsonStringExtensions
    {
        /// <summary>
        /// Use a geometry factory to convert a multipolygon geojson string into a multipolygon.
        /// NOTE: this requires that the keys 'type' and 'coordinates' be all-lowercase.
        /// </summary>
        public static MultiPolygon ToMultiPolygon(this GeoJsonMultiPolygonJsonString geoJsonMultiPolygonJsonString, IGeometryFactory geometryFactory)
        {
            // Deserialize string to a JObject.
            var jObject = JObject.Parse(geoJsonMultiPolygonJsonString.Value);

            // Test that the type *IS* multipolygon!
            var geometryTypeValue = jObject["type"].Value<string>();
            if(geometryTypeValue.ToLowerInvariant() != "multipolygon")
            {
                throw new ArgumentException("Input GeoJSON is not a multipolygon.", nameof(GeoJsonMultiPolygonJsonString));
            }

            // Create the linear rings.
            var coordinatesToken = jObject["coordinates"];

            var polygons = new List<IPolygon>();
            foreach (var polygonValues in coordinatesToken.AsJEnumerable())
            {
                ILinearRing shell = null;
                var holes = new List<ILinearRing>();
                var isFirst = true;
                foreach (var linearRingValues in polygonValues.AsJEnumerable())
                {
                    var coordinateValues = linearRingValues.ToObject<double[][]>();

                    var coordinates = new List<Coordinate>();
                    foreach (var coordinateValue in coordinateValues)
                    {
                        // X is longitude.
                        var coordinate = new Coordinate(coordinateValue[0], coordinateValue[1]);

                        coordinates.Add(coordinate);
                    }

                    var linearRing = geometryFactory.CreateLinearRing(coordinates.ToArray());

                    if (isFirst)
                    {
                        isFirst = false;

                        // The first linear ring is assumed to be the outside shell and must be counter-clockwise for the SQL server database (to enclose a limited space, instead of the whole world but the space).
                        shell = linearRing.IsCCW ? linearRing : linearRing.Reverse() as LinearRing;
                    }
                    else
                    {
                        // Holes must be clockwise for the SQL server database.
                        var hole = linearRing.IsCCW ? linearRing.Reverse() as LinearRing : linearRing;

                        holes.Add(hole);
                    }
                }

                // Create the polygon.
                var polygon = geometryFactory.CreatePolygon(shell, holes.ToArray());

                polygons.Add(polygon);
            }

            // Create the multipolygon.
            var multiPolygon = geometryFactory.CreateMultiPolygon(polygons.ToArray()) as MultiPolygon;
            return multiPolygon;
        }
    }
}
