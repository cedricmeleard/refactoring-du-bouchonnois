using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Builders;

public class ChasseurBuilder
{
    private readonly string _name;
    private int _nbBallesRestantes;
    private int _nbGalinettes;

    public ChasseurBuilder(string name, int nbBallesRestantes)
    {
        _name = name;
        _nbBallesRestantes = nbBallesRestantes;
    }

    public ChasseurBuilder AvecDesBallesRestantes(int nbBallesRestantes)
    {
        _nbBallesRestantes = nbBallesRestantes;
        return this;
    }

    public ChasseurBuilder AvecDesGalinettes(int nbGalinettes)
    {
        _nbGalinettes = nbGalinettes;
        return this;
    }

    public Chasseur Build()
    {
        return new Chasseur(_name!) { BallesRestantes = _nbBallesRestantes, NbGalinettes = _nbGalinettes };
    }

    public static ChasseurBuilder Dédé()
    {
        return new ChasseurBuilder("Dédé", 20);
    }

    public static ChasseurBuilder Bernard()
    {
        return new ChasseurBuilder("Bernard", 8);
    }

    public static ChasseurBuilder Robert()
    {
        return new ChasseurBuilder("Robert", 12);
    }
}