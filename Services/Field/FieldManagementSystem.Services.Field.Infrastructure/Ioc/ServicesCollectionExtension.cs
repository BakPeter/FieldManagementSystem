using FieldManagementSystem.Services.Field.Core.Interfaces;
using FieldManagementSystem.Services.Field.Core.Interfaces.Repository;
using FieldManagementSystem.Services.Field.Core.Interfaces.Validation;
using FieldManagementSystem.Services.Field.Core.Types.Repository.EF;
using FieldManagementSystem.Services.Field.Infrastructure.Services;
using FieldManagementSystem.Services.Field.Infrastructure.Services.Repository;
using FieldManagementSystem.Services.Field.Infrastructure.Services.Repository.EF;
using FieldManagementSystem.Services.Field.Infrastructure.Services.Validation;
using FieldManagementSystem.Services.Field.Infrastructure.Services.Validation.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FieldManagementSystem.Services.Field.Infrastructure.Ioc;

public static class ServicesCollectionExtension
{
    public static IServiceCollection AddFieldServices(this IServiceCollection services, EfRepositoryAdapterSettings pgSettings)
    {
        services.AddScoped<IFieldService, FieldService>();
        services.AddScoped<IFieldRepository, FieldRepository>();
        // services.AddSingleton<IFieldRepositoryAdapter, FieldCachedRepositoryAdapter>();
        services.AddScoped<IFieldRepositoryAdapter, EfRepositoryAdapter>();
        services.AddScoped<IFieldValidation, FieldValidation>();
        services.AddScoped<IFieldValidator, FieldNameValidator>();

        // EF
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(pgSettings.ConnectionString));

        return services;
    }
}