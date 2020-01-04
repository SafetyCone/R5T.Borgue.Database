using System;
using System.Linq;

using R5T.Corcyra;

using CatchmentEntity = R5T.Borgue.Database.Entities.Catchment;


namespace R5T.Borgue.Database
{
    public static class ICatchmentsDbContextExtensions
    {
        public static IQueryable<CatchmentEntity> GetCatchment(this ICatchmentsDbContext dbContext, CatchmentIdentity catchmentIdentity)
        {
            var catchmentQueryable = dbContext.Catchments.Where(x => x.Identity == catchmentIdentity.Value);
            return catchmentQueryable;
        }
    }
}