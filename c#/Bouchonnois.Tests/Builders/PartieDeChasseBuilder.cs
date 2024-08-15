using Bouchonnois.Domain;

namespace Bouchonnois.Tests.Builders;

public class PartieDeChasseBuilder
{
    private readonly List<ChasseurBuilder> _chasseurs;
    private readonly List<Event> _events;

    private readonly List<PartieStatus> _status = new();
    private Terrain? _terrain;

    public PartieDeChasseBuilder()
    {
        _events = new List<Event>();
        _chasseurs = new List<ChasseurBuilder>();
    }

    public static PartieDeChasseBuilder NouvellePartieDeChasse => new();

    public PartieDeChasseBuilder AvecUnTerrainRicheEnGalinettes(int nbGalinettes)
    {
        _terrain = new Terrain(Data.TerrainName) { NbGalinettes = nbGalinettes };
        return this;
    }

    public PartieDeChasseBuilder Avec(params ChasseurBuilder[] chasseurs)
    {
        foreach (var chasseur in chasseurs) _chasseurs.Add(chasseur);
        return this;
    }

    public PartieDeChasseBuilder AlorsQueLaPartieEst(PartieStatus status)
    {
        _status.Add(status);
        return this;
    }

    public PartieDeChasseBuilder WithAnEventLog(DateTime dateTime, string eventDescription)
    {
        _events.Add(new Event(dateTime, eventDescription));
        return this;
    }

    public PartieDeChasseBuilder WithEventLogs(params (DateTime date, string message)[] events)
    {
        foreach ((var date, string message) in events) WithAnEventLog(date, message);

        return this;
    }

    public PartieDeChasse Build(Func<DateTime> timeProvider, IPartieDeChasseRepository repository)
    {
        var builtChasseurs = _chasseurs.Select(c => c.Build()).ToList();
        var chasseursSansBalles = _chasseurs.Where(c => c.SansBalles).Select(c => c.Build().Nom).ToList();

        var partieDeChasse = PartieDeChasse.Create(
            timeProvider,
            (_terrain!.Nom, _terrain.NbGalinettes),
            builtChasseurs.Select(c => (c.Nom, c.BallesRestantes)).ToList());

        TirerSurLesGalinettes(timeProvider, repository, partieDeChasse, builtChasseurs);
        TirerDansLeVide(timeProvider, repository, chasseursSansBalles, partieDeChasse);
        ChangeStatus(partieDeChasse, timeProvider);

        return partieDeChasse;
    }
    private void ChangeStatus(PartieDeChasse partieDeChasse, Func<DateTime> timeProvider)
        => _status.ForEach(status => ChangerStatus(partieDeChasse, status, timeProvider));
    private void ChangerStatus(PartieDeChasse partieDeChasse, PartieStatus status, Func<DateTime> timeProvider)
    {
        if (status == PartieStatus.Apéro) {
            partieDeChasse.StartApero(timeProvider);
        } else if (status == PartieStatus.Terminée) {
            partieDeChasse.Terminer(timeProvider);
        }
    }


    private static void TirerDansLeVide(
        Func<DateTime> timeProvider,
        IPartieDeChasseRepository repository,
        List<string> chasseursSansBalles,
        PartieDeChasse partieDeChasse) => chasseursSansBalles.ForEach(c => partieDeChasse.Tirer(c, timeProvider, repository));

    private static void TirerSurLesGalinettes(
        Func<DateTime> timeProvider,
        IPartieDeChasseRepository repository,
        PartieDeChasse partieDeChasse,
        List<Chasseur> builtChasseurs)
    {
        partieDeChasse.Chasseurs
            .ToList()
            .ForEach(c =>
            {
                var built = builtChasseurs.First(x => x.Nom == c.Nom);
                int repeat = built.NbGalinettes;
                while (repeat > 0) {
                    partieDeChasse.TirerSurUneGalinette(built.Nom, timeProvider, repository);
                    repeat--;
                }
            });
    }
}