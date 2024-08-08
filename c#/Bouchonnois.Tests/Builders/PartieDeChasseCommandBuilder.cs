namespace Bouchonnois.Tests.Builders;

internal class PartieDeChasseCommandBuilder
{
    private readonly List<(string, int)> _chasseurs;
    public PartieDeChasseCommandBuilder()
    {
        _chasseurs = new List<(string, int)>();
    }

    public PartieDeChasseCommandBuilder Avec(params (string, int)[] chasseurs)
    {
        foreach (var chasseur in chasseurs) {
            _chasseurs.Add(chasseur);
        }
        return this;
    }
    public PartieDeChasseCommand SurUnTerrainRicheEnGalinettes(int nbGalinettes = 3)
    {
        return new PartieDeChasseCommand
        {
            Chasseurs = _chasseurs.ToList(),
            Terrain = ("Pitibon sur Sauldre", nbGalinettes)
        };
    }
}