using Microsoft.Extensions.DependencyInjection;
using Repository.Core.Interfaces;
using Repository.Core.Interfaces.Validation;
using Repository.Infrastructure.Services;

namespace Repository.Infrastructure.Ioc;

public static class ServicesCollectionExtension
{
    public static IServiceCollection AddRepositoryServices<TEntity>(this IServiceCollection services) where TEntity : class
    {
        services.AddTransient(typeof(IEntityValidation<TEntity>), typeof(EntityValidation<TEntity>));
        services.AddTransient(typeof(IRepository<TEntity>), typeof(Repository<TEntity>));
        return services;
    }
}