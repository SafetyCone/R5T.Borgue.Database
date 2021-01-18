using System;
using System.IO;

using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;

using R5T.Corcyra;


namespace R5T.Borgue.Database
{
    public static class MultiPolygonExtensions
    {
        public static GeoJsonMultiPolygonJsonString ToGeoJsonMultiPolygonJsonString(this MultiPolygon multiPolygon)
        {
            var geoJsonSerializer = GeoJsonSerializer.Create();
            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                geoJsonSerializer.Serialize(jsonWriter, multiPolygon);

                var geoJsonMultiPolygonJsonStringValue = stringWriter.ToString();

                var geoJsonMultiPolygonJsonString = GeoJsonMultiPolygonJsonString.From(geoJsonMultiPolygonJsonStringValue);
                return geoJsonMultiPolygonJsonString;
            }
        }
    }
}
