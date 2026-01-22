using FieldManagementSystem.User.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Repository.Core.Interfaces;
using Repository.Core.Interfaces.Validation;
using Repository.Infrastructure.Ioc;

namespace FieldManagementSystem.User.Infrastructure.Ioc;

public static class ServicesCollectionExtension
{
    public static IServiceCollection AddUserServices(this IServiceCollection services)
    {

        services.AddRepositoryServices<Core.Types.User>();
        services.AddSingleton<IRepositoryAdapter<Core.Types.User>, UserCachedRepositoryAdapter>();
        services.AddSingleton<IEntityValidator<Core.Types.User>, UserValidator>();

        return services;
    } 
}