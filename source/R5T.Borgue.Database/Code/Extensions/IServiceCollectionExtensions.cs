using System;

using Microsoft.Extensions.DependencyInjection;

using Microsoft.EntityFrameworkCore;

using R5T.Dacia;


namespace R5T.Borgue.Database
{
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="DatabaseCatchmentsRepository{TDbContext}"/> implementation of <see cref="ICatchmentsRepository"/> as <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddDatabaseCatchmentsRepository<TDbContext>(this IServiceCollection services,
            IServiceAction<IGeometryFactoryProvider> geometryFactoryProviderAction)
            where TDbContext: DbContext, ICatchmentsDbContext
        {
            services
                .AddSingleton<ICatchmentsRepository, DatabaseCatchmentsRepository<TDbContext>>()
                .Run(geometryFactoryProviderAction)
                ;

            return services;
        }

        /// <summary>
        /// Adds the <see cref="DatabaseCatchmentsRepository{TDbContext}"/> implementation of <see cref="ICatchmentsRepository"/> as <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<ICatchmentsRepository> AddDatabaseCatchmentsRepositoryAction<TDbContext>(this IServiceCollection services,
            IServiceAction<IGeometryFactoryProvider> geometryFactoryProviderAction)
            where TDbContext : DbContext, ICatchmentsDbContext
        {
            var serviceAction = ServiceAction.New<ICatchmentsRepository>(() => services.AddDatabaseCatchmentsRepository<TDbContext>(
                geometryFactoryProviderAction));

            return serviceAction;
        }

        /// <summary>
        /// Adds the <see cref="DatabaseCatchmentsRepository{TDbContext}"/> implementation of <see cref="ICatchmentsRepository"/> as <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceCollection AddDatabaseCatchmentsRepository<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext, ICatchmentsDbContext
        {
            var geometryFactoryProviderAction = services.AddGeometryFactoryProviderAction();

            services.AddDatabaseCatchmentsRepository<TDbContext>(geometryFactoryProviderAction);

            return services;
        }

        /// <summary>
        /// Adds the <see cref="DatabaseCatchmentsRepository{TDbContext}"/> implementation of <see cref="ICatchmentsRepository"/> as <see cref="ServiceLifetime.Singleton"/>.
        /// </summary>
        public static IServiceAction<ICatchmentsRepository> AddDatabaseCatchmentsRepositoryAction<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext, ICatchmentsDbContext
        {
            var serviceAction = ServiceAction.New<ICatchmentsRepository>(() => services.AddDatabaseCatchmentsRepository<TDbContext>());

            return serviceAction;
        }

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
