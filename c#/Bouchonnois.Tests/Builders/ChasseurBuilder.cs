using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Service;

public class ChasseurBuilder
{
    private string _name;
    private int _nbBallesRestantes;
    private int _nbGalinettes;

    public ChasseurBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    public ChasseurBuilder WithBallesRestantes(int nbBallesRestantes)
    {
        _nbBallesRestantes = nbBallesRestantes;
        return this;
    }

    public ChasseurBuilder WithGalinettes(int nbGalinettes)
    {
        _nbGalinettes = nbGalinettes;
        return this;
    }

    public Chasseur Build()
    {
        return new Chasseur(_name) { BallesRestantes = _nbBallesRestantes, NbGalinettes = _nbGalinettes };
    }
}