using System;
using System.Threading.Tasks;

using GeoAPI.Geometries;

using R5T.T0064;


namespace R5T.Borgue.Database
{
    [ServiceDefinitionMarker]
    public interface IGeometryFactoryProvider : IServiceDefinition
    {
        Task<IGeometryFactory> GetGeometryFactoryAsync();
    }
}
