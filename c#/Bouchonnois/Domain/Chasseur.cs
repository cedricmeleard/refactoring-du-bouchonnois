namespace Bouchonnois.Domain;

public class Chasseur(string nom, int nbBalles)
{
    public string Nom { get; } = nom;
    public int BallesRestantes { get; private set; } = nbBalles;
    public int NbGalinettes { get; private set; }
    public bool AEncoreDesBalles() => BallesRestantes != 0;
    public void ATiré()
    {
        BallesRestantes--;
    }
    public void ATuéUneGalinette()
    {
        NbGalinettes++;
    }
}