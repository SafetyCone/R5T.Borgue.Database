using System;
using System.Threading.Tasks;

using GeoAPI.Geometries;
using NetTopologySuite;

using R5T.T0064;


namespace R5T.Borgue.Database
{
    [ServiceImplementationMarker]
    public class GeometryFactoryProvider : IGeometryFactoryProvider, IServiceImplementation
    {
        private static IGeometryFactory GeometryFactory { get; } = NtsGeometryServices.Instance.CreateGeometryFactory(srid: Constants.StandardSRID); // Recommended SRID, used by Google Maps.


        public Task<IGeometryFactory> GetGeometryFactoryAsync()
        {
            return Task.FromResult(GeometryFactoryProvider.GeometryFactory);
        }
    }
}
