using System;

using Microsoft.EntityFrameworkCore;


namespace R5T.Borgue.Database
{
    public interface IGridUnitsDbContext
    {
        DbSet<Entities.GridUnit> GridUnits { get; }
        DbSet<Entities.CatchmentGridUnit> CatchmentGridUnits { get; }
    }
}
