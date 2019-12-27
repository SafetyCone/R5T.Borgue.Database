using System;

using Microsoft.EntityFrameworkCore;


namespace R5T.Borgue.Database
{
    public interface ICatchmentsDbContext
    {
        DbSet<Entities.Catchment> Catchments { get; set; }
    }
}
