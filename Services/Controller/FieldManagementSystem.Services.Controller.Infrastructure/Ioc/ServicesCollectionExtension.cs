using FieldManagementSystem.Services.Controller.Core.Interfaces;
using FieldManagementSystem.Services.Controller.Core.Interfaces.Repository;
using FieldManagementSystem.Services.Controller.Core.Interfaces.Validation;
using FieldManagementSystem.Services.Controller.Infrastructure.Services;
using FieldManagementSystem.Services.Controller.Infrastructure.Services.Repository;
using FieldManagementSystem.Services.Controller.Infrastructure.Services.Validation;
using FieldManagementSystem.Services.Controller.Infrastructure.Services.Validation.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace FieldManagementSystem.Services.Controller.Infrastructure.Ioc;

public static class ServicesCollectionExtension
{
    public static IServiceCollection AddControllerServices(this IServiceCollection services)
    {
        services.AddTransient<IControllerService, ControllerService>();
        services.AddSingleton<IControllerRepository, ControllerRepository>();
        services.AddSingleton<IControllerRepositoryAdapter, ControllerCachedRepositoryAdapter>();
        services.AddSingleton<IControllerValidation, ControllerValidation>();
        
        return services;
    }
}