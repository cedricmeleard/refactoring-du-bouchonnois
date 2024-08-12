using Bouchonnois.Domain;
using Bouchonnois.Tests.Builders;
using Bouchonnois.Tests.Doubles;
using FsCheck;
using Microsoft.FSharp.Collections;

namespace Bouchonnois.Tests.Unit;

public abstract class PartieDeChasseServiceTest
{
    protected readonly static DateTime Now = new(2024, 6, 6, 14, 50, 45);
    protected readonly static Func<DateTime> TimeProvider = () => Now;
    protected readonly PartieDeChasseRepositoryForTests Repository;
    protected PartieDeChasseServiceTest()
    {
        Repository = new PartieDeChasseRepositoryForTests();
    }
    protected ChasseurBuilder Dédé => Dédé();
    protected ChasseurBuilder Bernard => Bernard();
    protected ChasseurBuilder Robert => Robert();
    protected PartieDeChasseBuilder NouvellePartieDeChasse => new();
    protected IEnumerable<(string nom, int nbBalles)> PasDeChasseurs => [];

    protected PartieDeChasseCommandBuilder DémarrerUnePartieDeChasse() => new();

    protected static void AssertLastEvent(PartieDeChasse partieDeChasse, string expectedMessage)
    {
        partieDeChasse
            .Events.Should()
            .HaveCount(1)
            .And
            .EndWith(new Event(Now, expectedMessage));
    }

    protected PartieDeChasse AvecUnePartieDeChasseExistante(PartieDeChasseBuilder partieDeChasseBuilder)
    {
        var partieDeChasse = partieDeChasseBuilder.Build();
        Repository.Add(partieDeChasse);

        return partieDeChasse;
    }

    protected bool MustFailWith<TException>(Action action, Func<PartieDeChasse?, bool>? assert = null)
        where TException : Exception
    {
        try {
            action();
            return false;
        }
        catch (TException) {
            return assert?.Invoke(Repository.SavedPartieDeChasse()) ?? true;
        }
    }

    protected static Arbitrary<FSharpList<(string nom, int nbBalles)>> GroupeDeChasseursAvecBallesGenerator()
        => GroupeDeChasseursGenerator(1, int.MaxValue);

    protected Arbitrary<FSharpList<(string nom, int nbBalles)>> GroupeDeChasseursSansBalleGenerator()
        => GroupeDeChasseursGenerator(0, 0);

    protected static Arbitrary<(string nom, int nbGalinettes)> TerrainAvecGalinettesGenerator()
        // A minima 1 galinette sur le terrain
        => TerrainGenerator(1, int.MaxValue);

    protected static Arbitrary<(string nom, int nbGalinettes)> TerrainSansGalinetteGenerator()
        => TerrainGenerator(0, 0);

    private static Arbitrary<(string nom, int nbGalinettes)> TerrainGenerator(int nbGalinettesMin, int nbGalinettesMax)
        // A minima 1 galinette sur le terrain
        => (from nom in Arb.Generate<string>()
            from nbGalinette in Gen.Choose(nbGalinettesMin, nbGalinettesMax)
            select (nom, nbGalinette)).ToArbitrary();

    private static Arbitrary<(string nom, int nbBalles)> ChasseurGenerator(int minBalles, int maxBalles)
        // A minima 1 balle
        => (from nom in Arb.Generate<string>()
            from nbBalles in Gen.Choose(minBalles, maxBalles)
            select (nom, nbBalles)).ToArbitrary();

    private static Arbitrary<FSharpList<(string nom, int nbBalles)>> GroupeDeChasseursGenerator(int minBalles, int maxBalles)
        // On définit le nombre de chasseurs dans le groupe [1; 1000]
        => (from nbChasseurs in Gen.Choose(1, 1_000)
            // On utilise le nombre de chasseurs pour générer le bon nombre de chasseurs
            select ChasseurGenerator(minBalles, maxBalles).Generator.Sample(1, nbChasseurs)).ToArbitrary();
}