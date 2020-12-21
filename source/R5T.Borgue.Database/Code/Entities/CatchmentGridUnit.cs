using System;
using System.ComponentModel.DataAnnotations;

namespace R5T.Borgue.Database.Entities
{
    public class CatchmentGridUnit
    {
        public int ID { get; set; }

        [Required]
        public Guid CatchmentIdentity { get; set; }
        [Required]
        public Guid GridUnitIdentity { get; set; }
    }
}
