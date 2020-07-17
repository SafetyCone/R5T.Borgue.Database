using System;
using System.Threading.Tasks;

using GeoAPI.Geometries;


namespace R5T.Borgue.Database
{
    public interface IGeometryFactoryProvider
    {
        Task<IGeometryFactory> GetGeometryFactoryAsync();
    }
}
