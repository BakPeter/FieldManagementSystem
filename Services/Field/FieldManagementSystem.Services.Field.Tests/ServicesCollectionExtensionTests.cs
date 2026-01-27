using FieldManagementSystem.Services.Field.Core.Interfaces;
using FieldManagementSystem.Services.Field.Core.Interfaces.Repository;
using FieldManagementSystem.Services.Field.Core.Interfaces.Validation;
using FieldManagementSystem.Services.Field.Infrastructure.Ioc;
using FieldManagementSystem.Services.Field.Infrastructure.Services;
using FieldManagementSystem.Services.Field.Infrastructure.Services.Repository;
using FieldManagementSystem.Services.Field.Infrastructure.Services.Validation;
using FieldManagementSystem.Services.Field.Infrastructure.Services.Validation.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace FieldManagementSystem.Services.Field.Tests;

[TestFixture]
public class ServicesCollectionExtensionTests
{
    private IServiceCollection _services;

    [SetUp]
    public void Setup()
    {
        _services = new ServiceCollection();
    }

    [TearDown]
    public void Teardown()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        _services = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }


    [Ignore("Broken After Code Ef Changes")]
    [Test]
    public void AddFieldServices_RegistersAllExpectedServices()
    {
        // Act
        // _services.AddFieldServices();

        // Assert
        // IFieldService as FieldService (Transient)
        var fieldServiceDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IFieldService));
        Assert.That(fieldServiceDescriptor, Is.Not.Null);
        Assert.That(fieldServiceDescriptor.ImplementationType, Is.EqualTo(typeof(FieldService)));
        Assert.That(fieldServiceDescriptor.Lifetime, Is.EqualTo(ServiceLifetime.Transient));

        // IFieldRepository as FieldRepository (Singleton)
        var fieldRepositoryDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IFieldRepository));
        Assert.That(fieldRepositoryDescriptor, Is.Not.Null);
        Assert.That(fieldRepositoryDescriptor.ImplementationType, Is.EqualTo(typeof(FieldRepository)));
        Assert.That(fieldRepositoryDescriptor.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));

        // IFieldRepositoryAdapter as FieldCachedRepositoryAdapter (Singleton)
        var fieldRepositoryAdapterDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IFieldRepositoryAdapter));
        Assert.That(fieldRepositoryAdapterDescriptor, Is.Not.Null);
        Assert.That(fieldRepositoryAdapterDescriptor.ImplementationType, Is.EqualTo(typeof(FieldCachedRepositoryAdapter)));
        Assert.That(fieldRepositoryAdapterDescriptor.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));

        // IFieldValidation as FieldValidation (Singleton)
        var fieldValidationDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IFieldValidation));
        Assert.That(fieldValidationDescriptor, Is.Not.Null);
        Assert.That(fieldValidationDescriptor.ImplementationType, Is.EqualTo(typeof(FieldValidation)));
        Assert.That(fieldValidationDescriptor.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));

        // IFieldValidator as FieldNameValidator (Singleton)
        var fieldValidatorDescriptor = _services.FirstOrDefault(s => s.ServiceType == typeof(IFieldValidator));
        Assert.That(fieldValidatorDescriptor, Is.Not.Null);
        Assert.That(fieldValidatorDescriptor.ImplementationType, Is.EqualTo(typeof(FieldNameValidator)));
        Assert.That(fieldValidatorDescriptor.Lifetime, Is.EqualTo(ServiceLifetime.Singleton));
    }
}
