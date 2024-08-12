using ArchUnitNET.Fluent;
using ArchUnitNET.Fluent.Syntax.Elements.Types;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using Bouchonnois.UseCases;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Bouchonnois.Tests.Architecture;

public static class ArchUnitExtensions
{
    private readonly static ArchUnitNET.Domain.Architecture Architecture =
        new ArchLoader()
            .LoadAssemblies(typeof(DemarrerUnePartieDeChasseUseCase).Assembly)
            .Build();

    public static GivenTypesConjunction TypesInAssembly() =>
        Types().That().Are(Architecture.Types);

    public static void Check(this IArchRule rule) => rule.Check(Architecture);
}