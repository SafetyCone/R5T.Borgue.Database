using System;
using System.Threading.Tasks;

using GeoAPI.Geometries;
using NetTopologySuite;


namespace R5T.Borgue.Database
{
    public class GeometryFactoryProvider : IGeometryFactoryProvider
    {
        private static IGeometryFactory GeometryFactory { get; } = NtsGeometryServices.Instance.CreateGeometryFactory(srid: Constants.StandardSRID); // Recommended SRID, used by Google Maps.


        public Task<IGeometryFactory> GetGeometryFactoryAsync()
        {
            return Task.FromResult(GeometryFactoryProvider.GeometryFactory);
        }
    }
}
