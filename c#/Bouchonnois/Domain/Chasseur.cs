namespace Bouchonnois.Domain
{
    public class Chasseur(string nom)
    {
        public string Nom { get; set; } = nom;
        public int BallesRestantes { get; set; }
        public int NbGalinettes { get; set; }
    }
}