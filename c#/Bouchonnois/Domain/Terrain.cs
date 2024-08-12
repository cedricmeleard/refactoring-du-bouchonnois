namespace Bouchonnois.Domain;

public class Terrain(string nom)
{
    public string Nom { get; } = nom;
    public int NbGalinettes { get; set; }
}