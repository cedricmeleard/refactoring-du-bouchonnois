namespace Bouchonnois.Domain;

public class Terrain(string nom, int nbGalinettes)
{
    public string Nom { get; } = nom;
    public int NbGalinettes { get; private set; } = nbGalinettes;
    public void UneGalinetteEnMoins()
    {
        NbGalinettes--;
    }
}