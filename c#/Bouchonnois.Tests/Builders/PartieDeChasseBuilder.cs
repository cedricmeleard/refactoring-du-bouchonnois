using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Builders;

public class PartieDeChasseBuilder
{
    private readonly List<ChasseurBuilder> _chasseurs;
    private readonly List<Event> _events;
    private readonly Guid _id;
    
    private PartieStatus _status = PartieStatus.EnCours;
    private Terrain? _terrain;
    public PartieDeChasseBuilder()
    {
        _id = Guid.NewGuid();
        _events = new List<Event>();
        _chasseurs = new List<ChasseurBuilder>();
    }

    public PartieDeChasseBuilder AvecUnTerrainRicheEnGalinettes(int nbGalinettes)
    {
        _terrain = new Terrain(Data.TerrainName) { NbGalinettes = nbGalinettes };
        return this;
    }

    public PartieDeChasseBuilder Avec(params ChasseurBuilder[] chasseurs)
    {
        foreach (var chasseur in chasseurs) {
            _chasseurs.Add(chasseur);
        }
        return this;
    }

    public PartieDeChasseBuilder AlorsQueLaPartieEst(PartieStatus status)
    {
        _status = status;
        return this;
    }

    public PartieDeChasseBuilder WithAnEventLog(DateTime dateTime, string eventDescription)
    {
        _events.Add(new Event(dateTime, eventDescription));
        return this;
    }

    public PartieDeChasseBuilder WithEventLogs(params (DateTime date, string message)[] events)
    {
        foreach ((var date, string message) in events) {
            WithAnEventLog(date, message);
        }

        return this;
    }

    public PartieDeChasse Build()
    {
        return new PartieDeChasse(_id, _terrain!)
        {
            Chasseurs = _chasseurs.Select(p => p.Build()).ToList(),
            Status = _status,
            Events = _events
        };
    }
}