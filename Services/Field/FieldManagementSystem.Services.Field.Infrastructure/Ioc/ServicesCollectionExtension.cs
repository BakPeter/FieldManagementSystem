using FieldManagementSystem.Services.Field.Core.Interfaces;
using FieldManagementSystem.Services.Field.Core.Interfaces.Repository;
using FieldManagementSystem.Services.Field.Core.Interfaces.Validation;
using FieldManagementSystem.Services.Field.Infrastructure.Services;
using FieldManagementSystem.Services.Field.Infrastructure.Services.Repository;
using FieldManagementSystem.Services.Field.Infrastructure.Services.Validation;
using FieldManagementSystem.Services.Field.Infrastructure.Services.Validation.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace FieldManagementSystem.Services.Field.Infrastructure.Ioc;

public static class ServicesCollectionExtension
{
    public static IServiceCollection AddFieldServices(this IServiceCollection services)
    {
        services.AddTransient<IFieldService, FieldService>();
        services.AddSingleton<IFieldRepository, FieldRepository>();
        services.AddSingleton<IFieldRepositoryAdapter, FieldCachedRepositoryAdapter>();
        services.AddSingleton<IFieldValidation, FieldValidation>();
        services.AddSingleton<IFieldValidator, FieldNameValidator>();
        return services;
    }
}