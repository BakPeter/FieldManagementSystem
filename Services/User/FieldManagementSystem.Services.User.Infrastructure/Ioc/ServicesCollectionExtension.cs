using FieldManagementSystem.User.Core.Interfaces;
using FieldManagementSystem.User.Core.Interfaces.Repository;
using FieldManagementSystem.User.Core.Interfaces.Validation;
using FieldManagementSystem.User.Infrastructure.Services;
using FieldManagementSystem.User.Infrastructure.Services.Repository;
using FieldManagementSystem.User.Infrastructure.Services.Validation;
using FieldManagementSystem.User.Infrastructure.Services.Validation.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace FieldManagementSystem.User.Infrastructure.Ioc;

public static class ServicesCollectionExtension
{
    public static IServiceCollection AddUserServices(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<IUserRepositoryAdapter, UserCachedRepositoryAdapter>();

        services.AddSingleton<IUserValidation, UserValidation>();
        services.AddSingleton<IUserValidator, UserEmailValidator>();
        services.AddSingleton<IUserValidator, UserNameValidator>();

        return services;
    }
}