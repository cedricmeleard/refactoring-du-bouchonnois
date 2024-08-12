using ArchUnitNET.Fluent.Syntax.Elements.Types;
using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Architecture;

public class ArchitectureRules
{
    [Fact]
    public void ApplicationServicesRules()
    {
        // Les classes dans l'Application Services ne devraient pas dépendre de classes dans InfrastructureDomainModel()
        ApplicationServices()
            .Should()
            .NotDependOnAny(Infrastructure())
            .Check();
    }

    [Fact]
    public void InfrastructureRules()
    {
        // Quelles sont les classes de l'infrastructure ?
        // Que devrions nous faire de ce qui est contenu dans Infra ?
        Infrastructure().Should()
            .ImplementInterface(typeof(IPartieDeChasseRepository))
            .Check();
    }


    [Fact]
    public void DomainModelRules()
    {
        // Les classes dans Domain ne devraient pas dépendre de classes dans Infrastructure ou Application Services
        DomainModel()
            .Should()
            .NotDependOnAny(ApplicationServices()).AndShould()
            .NotDependOnAny(Infrastructure())
            .Check();
    }

    private static GivenTypesConjunctionWithDescription ApplicationServices() =>
        ArchUnitExtensions.TypesInAssembly().And()
            .ResideInNamespace("Service", true)
            .As("Application Services");

    private static GivenTypesConjunctionWithDescription DomainModel() =>
        ArchUnitExtensions.TypesInAssembly().And()
            .ResideInNamespace("Domain", true)
            .As("Domain Model");

    private static GivenTypesConjunctionWithDescription Infrastructure() =>
        ArchUnitExtensions.TypesInAssembly().And()
            .ResideInNamespace("Repository", true)
            .As("Infrastructure");
}