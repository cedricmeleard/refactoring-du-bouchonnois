using Bouchonnois.Domain;
using Bouchonnois.Service;

namespace Bouchonnois.Tests.Service;

public class PartieDeChasseBuilder
{
    private readonly List<Chasseur> _chasseurs;
    private readonly List<Event> _events;

    private Guid _id;
    private PartieStatus _status = PartieStatus.EnCours;
    private Terrain _terrain;
    public PartieDeChasseBuilder()
    {
        _events = new List<Event>();
        _chasseurs = new List<Chasseur>();
    }
    public PartieDeChasseBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }
    public PartieDeChasseBuilder WithTerrain(string name, int nbGalinettes)
    {
        _terrain = new Terrain(name) { NbGalinettes = nbGalinettes };
        return this;
    }
    public PartieDeChasseBuilder WithChasseur(string name, int nbBallesRestantes)
    {
        var chasseur = new ChasseurBuilder()
            .WithName(name)
            .WithBallesRestantes(nbBallesRestantes)
            .Build();
        _chasseurs.Add(chasseur);
        return this;
    }
    public PartieDeChasseBuilder WithChasseurWithGalinettes(string name, int nbBallesRestantes, int nbGalinettes)
    {
        var chasseur = new ChasseurBuilder()
            .WithName(name)
            .WithBallesRestantes(nbBallesRestantes)
            .WithGalinettes(nbGalinettes)
            .Build();
        _chasseurs.Add(chasseur);
        return this;
    }

    public PartieDeChasseBuilder WithPartieStatus(PartieStatus status)
    {
        _status = status;
        return this;
    }

    public PartieDeChasseBuilder WithEvent(DateTime dateTime, string eventDescription)
    {
        _events.Add(new Event(dateTime, eventDescription));
        return this;
    }

    public PartieDeChasseBuilder WithEvents(params (DateTime date, string message)[] events)
    {
        foreach ((var date, string message) in events) {
            WithEvent(date, message);
        }

        return this;
    }

    public PartieDeChasse Build()
    {
        return new PartieDeChasse(_id, _terrain) { Chasseurs = _chasseurs, Status = _status, Events = _events };
    }
}