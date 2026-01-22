using FieldManagementSystem.Services.Repository.Core.Interfaces;
using FieldManagementSystem.Services.Repository.Core.Interfaces.Validation;
using FieldManagementSystem.Services.Repository.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FieldManagementSystem.Services.Repository.Infrastructure.Ioc;

public static class ServicesCollectionExtension
{
    public static IServiceCollection AddRepositoryServices<TEntity>(this IServiceCollection services) where TEntity : class
    {
        services.AddTransient(typeof(IEntityValidation<TEntity>), typeof(EntityValidation<TEntity>));
        services.AddTransient(typeof(IRepository<TEntity>), typeof(Repository<TEntity>));
        return services;
    }
}