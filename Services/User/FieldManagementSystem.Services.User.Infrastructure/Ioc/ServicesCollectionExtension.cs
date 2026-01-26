using FieldManagementSystem.Services.User.Core.Interfaces;
using FieldManagementSystem.Services.User.Core.Interfaces.Repository;
using FieldManagementSystem.Services.User.Core.Interfaces.Validation;
using FieldManagementSystem.Services.User.Infrastructure.Configurations.Repository.EF;
using FieldManagementSystem.Services.User.Infrastructure.Services;
using FieldManagementSystem.Services.User.Infrastructure.Services.Repository;
using FieldManagementSystem.Services.User.Infrastructure.Services.Repository.EF;
using FieldManagementSystem.Services.User.Infrastructure.Services.Validation;
using FieldManagementSystem.Services.User.Infrastructure.Services.Validation.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FieldManagementSystem.Services.User.Infrastructure.Ioc;

public static class ServicesCollectionExtension
{
    public static IServiceCollection AddUserServices(this IServiceCollection services, EfRepositoryAdapterSettings pgSettings)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRepository, UserRepository>();
        // services.AddSingleton<IUserRepositoryAdapter, UserCachedRepositoryAdapter>();
        services.AddScoped<IUserRepositoryAdapter, EfRepositoryAdapter>();

        services.AddScoped<IUserValidation, UserValidation>();
        services.AddScoped<IUserValidator, UserEmailValidator>();
        services.AddScoped<IUserValidator, UserNameValidator>();


        // EF
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(pgSettings.ConnectionString));
        return services;
    }
}
