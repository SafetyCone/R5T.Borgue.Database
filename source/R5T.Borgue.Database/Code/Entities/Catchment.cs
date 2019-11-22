using System;
//using System.ComponentModel.DataAnnotations.Schema;

using NetTopologySuite.Geometries;


namespace R5T.Borgue.Database.Entities
{
    public class Catchment
    {
        public int ID { get; set; }

        public Guid Identity { get; set; }

        //[Column(TypeName = "geography")] // This is the default for MS SQL.
        public Geometry Boundary { get; set; }
    }
}
