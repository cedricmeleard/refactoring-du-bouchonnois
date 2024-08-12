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

    public static ChasseurBuilder Dédé => new(Data.Dédé, 20);

    public static ChasseurBuilder Bernard => new(Data.Bernard, 8);

    public static ChasseurBuilder Robert => new(Data.Robert, 12);

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
}