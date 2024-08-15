using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Builders;

public class ChasseurBuilder
{
    private readonly string _name;
    private int _nbBallesRestantes;

    public ChasseurBuilder(string name, int nbBallesRestantes)
    {
        _name = name;
        _nbBallesRestantes = nbBallesRestantes;
    }
    public int NbGalinettes { get; private set; }

    public bool SansBalles { get; private set; }

    public static ChasseurBuilder Dédé => new(Data.Dédé, 20);

    public static ChasseurBuilder Bernard => new(Data.Bernard, 8);

    public static ChasseurBuilder Robert => new(Data.Robert, 12);

    public ChasseurBuilder SansBalle()
    {
        SansBalles = true;
        _nbBallesRestantes = 1;
        return this;
    }

    public ChasseurBuilder AyantTué(int nbGalinettes)
    {
        NbGalinettes = nbGalinettes;
        return this;
    }

    public Chasseur Build() => new(_name!, _nbBallesRestantes);
}