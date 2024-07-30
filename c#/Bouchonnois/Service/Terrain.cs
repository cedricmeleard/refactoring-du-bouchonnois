namespace Bouchonnois.Service
{
    public class Terrain(string nom)
    {
        public string Nom { get; init; } = nom;
        public int NbGalinettes { get; set; }
    }
}