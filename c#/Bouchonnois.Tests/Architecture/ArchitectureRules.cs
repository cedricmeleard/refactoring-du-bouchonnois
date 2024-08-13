using ArchUnitNET.Fluent.Syntax.Elements.Types;
using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Architecture;

public class ArchitectureRules
{
    [Fact]
    public void ApplicationServicesRules()
    {
        UseCases()
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
        DomainModel()
            .Should()
            .NotDependOnAny(UseCases()).AndShould()
            .NotDependOnAny(Infrastructure())
            .Check();
    }

    private static GivenTypesConjunctionWithDescription UseCases() =>
        ArchUnitExtensions.TypesInAssembly().And()
            .ResideInNamespace("UseCases", true)
            .As("Use Cases");

    private static GivenTypesConjunctionWithDescription DomainModel() =>
        ArchUnitExtensions.TypesInAssembly().And()
            .ResideInNamespace("Domain", true)
            .As("Domain Model");

    private static GivenTypesConjunctionWithDescription Infrastructure() =>
        ArchUnitExtensions.TypesInAssembly().And()
            .ResideInNamespace("Repository", true)
            .As("Infrastructure");
}