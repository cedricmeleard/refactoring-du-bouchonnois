using FsCheck;
using Microsoft.FSharp.Collections;

namespace Bouchonnois.Tests.Unit;

public static class Generators
{
    // A minima 1 balle pour un chasseur
    public static Arbitrary<FSharpList<(string nom, int nbBalles)>> GroupeDeChasseursAvecBallesGenerator() => GroupeDeChasseursGenerator(1, int.MaxValue);
    public static Arbitrary<FSharpList<(string nom, int nbBalles)>> GroupeDeChasseursSansBalleGenerator() => GroupeDeChasseursGenerator(0, 0);

    // A minima 1 galinette sur le terrain
    public static Arbitrary<(string nom, int nbGalinettes)> TerrainAvecGalinettesGenerator() => TerrainGenerator(1, int.MaxValue);
    public static Arbitrary<(string nom, int nbGalinettes)> TerrainSansGalinetteGenerator() => TerrainGenerator(0, 0);

    private static Arbitrary<(string nom, int nbGalinettes)> TerrainGenerator(int nbGalinettesMin, int nbGalinettesMax)
        => (from nom in Arb.Generate<string>()
            from nbGalinette in Gen.Choose(nbGalinettesMin, nbGalinettesMax)
            select (nom, nbGalinette)).ToArbitrary();
    private static Arbitrary<(string nom, int nbBalles)> ChasseurGenerator(int minBalles, int maxBalles)
        => (from nom in Arb.Generate<string>()
            from nbBalles in Gen.Choose(minBalles, maxBalles)
            select (nom, nbBalles)).ToArbitrary();
    private static Arbitrary<FSharpList<(string nom, int nbBalles)>> GroupeDeChasseursGenerator(int minBalles, int maxBalles)
        => (from nbChasseurs in Gen.Choose(1, 1_000) // On définit le nombre de chasseurs dans le groupe [1; 1000]
            select ChasseurGenerator(minBalles, maxBalles).Generator.Sample(1, nbChasseurs)).ToArbitrary();
}