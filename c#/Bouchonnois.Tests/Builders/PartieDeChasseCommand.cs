namespace Bouchonnois.Tests.Builders;

internal class PartieDeChasseCommand
{
    public PartieDeChasseCommand()
    {
        Chasseurs = new List<(string, int)>();
    }
    public List<(string, int)> Chasseurs { get; init; }
    public (string, int nbGalinettes) Terrain { get; init; }
}