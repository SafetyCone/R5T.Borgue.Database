using System;

using NetTopologySuite.Geometries;

namespace R5T.Borgue.Database.Entities
{
    public class GridUnit
    {
        public int ID { get; set; }
        public Guid GUID { get; set; }

        public string Name { get; set; }
        public Geometry Boundary { get; set; }
    }
}
