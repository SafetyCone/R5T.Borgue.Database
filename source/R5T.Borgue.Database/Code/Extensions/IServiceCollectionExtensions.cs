using System;

using Microsoft.Extensions.DependencyInjection;

using Microsoft.EntityFrameworkCore;

using R5T.Borgue;
using R5T.Dacia;


namespace R5T.Borgue.Database
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="DatabaseGridUnitsRepository{TDbContext}"/> implementation of <see cref="IGridUnitsRepository"/> as <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddDatabaseGridUnitsRepository<TDbContext>(this IServiceCollection services,
            IServiceAction<IGeometryFactoryProvider> geometryFactoryProviderAction)
            where TDbContext: DbContext, ICatchmentsDbContext, IGridUnitsDbContext
        {
            services
                .AddSingleton<IGridUnitsRepository, DatabaseGridUnitsRepository<TDbContext>>()
                .Run(geometryFactoryProviderAction)
                ;

            return services;
        }

        /// <summary>
        /// Adds the <see cref="DatabaseGridUnitsRepository{TDbContext}"/> implementation of <see cref="IGridUnitsRepository"/> as <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<IGridUnitsRepository> AddDatabaseGridUnitsRepositoryAction<TDbContext>(this IServiceCollection services,
            IServiceAction<IGeometryFactoryProvider> geometryFactoryProviderAction)
            where TDbContext : DbContext, ICatchmentsDbContext, IGridUnitsDbContext
        {
            var serviceAction = ServiceAction.New<IGridUnitsRepository>(() => services.AddDatabaseGridUnitsRepository<TDbContext>(
                geometryFactoryProviderAction));

            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="DatabaseGridUnitsRepository{TDbContext}"/> implementation of <see cref="IGridUnitsRepository"/> as <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddDatabaseGridUnitsRepository<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext, ICatchmentsDbContext, IGridUnitsDbContext
        {
            var geometryFactoryProviderAction = services.AddGeometryFactoryProviderAction();

            services.AddDatabaseGridUnitsRepository<TDbContext>(geometryFactoryProviderAction);

            return services;
        }

        /// <summary>
        /// Adds the <see cref="DatabaseGridUnitsRepository{TDbContext}"/> implementation of <see cref="IGridUnitsRepository"/> as <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<IGridUnitsRepository> AddDatabaseGridUnitsRepositoryAction<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext, ICatchmentsDbContext, IGridUnitsDbContext
        {
            var serviceAction = ServiceAction.New<IGridUnitsRepository>(() => services.AddDatabaseGridUnitsRepository<TDbContext>());

            return serviceAction;
        }

        // ------------------
        // GeometryFactoryProvider

        /// <summary>
        /// Adds the <see cref="DatabaseCatchmentsRepository{TDbContext}"/> implementation of <see cref="ICatchmentsRepository"/> as <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddDatabaseCatchmentsRepository<TDbContext>(this IServiceCollection services,
            IServiceAction<IGeometryFactoryProvider> geometryFactoryProviderAction,
            IServiceAction<IGridUnitsRepository> gridUnitsRepositoryAction)
            where TDbContext: DbContext, ICatchmentsDbContext, IGridUnitsDbContext
        {
            services
                .AddSingleton<ICatchmentsRepository, DatabaseCatchmentsRepository<TDbContext>>()
                .Run(geometryFactoryProviderAction)
                .Run(gridUnitsRepositoryAction)
                ;

            return services;
        }

        /// <summary>
        /// Adds the <see cref="DatabaseCatchmentsRepository{TDbContext}"/> implementation of <see cref="ICatchmentsRepository"/> as <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<ICatchmentsRepository> AddDatabaseCatchmentsRepositoryAction<TDbContext>(this IServiceCollection services,
            IServiceAction<IGeometryFactoryProvider> geometryFactoryProviderAction,
            IServiceAction<IGridUnitsRepository> gridUnitsRepositoryAction)
            where TDbContext : DbContext, ICatchmentsDbContext, IGridUnitsDbContext
        {
            var serviceAction = ServiceAction.New<ICatchmentsRepository>(() => services.AddDatabaseCatchmentsRepository<TDbContext>(
                geometryFactoryProviderAction, gridUnitsRepositoryAction));

            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="DatabaseCatchmentsRepository{TDbContext}"/> implementation of <see cref="ICatchmentsRepository"/> as <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddDatabaseCatchmentsRepository<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext, ICatchmentsDbContext, IGridUnitsDbContext
        {
            var geometryFactoryProviderAction = services.AddGeometryFactoryProviderAction();
            var gridUnitsRepositoryAction = services.AddDatabaseGridUnitsRepositoryAction<TDbContext>(geometryFactoryProviderAction);

            services.AddDatabaseCatchmentsRepository<TDbContext>(geometryFactoryProviderAction, gridUnitsRepositoryAction);

            return services;
        }

        /// <summary>
        /// Adds the <see cref="DatabaseCatchmentsRepository{TDbContext}"/> implementation of <see cref="ICatchmentsRepository"/> as <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<ICatchmentsRepository> AddDatabaseCatchmentsRepositoryAction<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext, ICatchmentsDbContext, IGridUnitsDbContext
        {
            var serviceAction = ServiceAction.New<ICatchmentsRepository>(() => services.AddDatabaseCatchmentsRepository<TDbContext>());

            return serviceAction;
        }

        // ------------------
        // GeometryFactoryProvider

        /// <summary>
        /// Adds the <see cref="GeometryFactoryProvider"/> implementation of <see cref="IGeometryFactoryProvider"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddGeometryFactoryProvider(this IServiceCollection services)
        {
            services.AddSingleton<IGeometryFactoryProvider, GeometryFactoryProvider>();

            return services;
        }

        /// <summary>
        /// Adds the <see cref="GeometryFactoryProvider"/> implementation of <see cref="IGeometryFactoryProvider"/> as a <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<IGeometryFactoryProvider> AddGeometryFactoryProviderAction(this IServiceCollection services)
        {
            var serviceAction = ServiceAction.New<IGeometryFactoryProvider>(() => services.AddGeometryFactoryProvider());
            return serviceAction;
        }
    }
}
