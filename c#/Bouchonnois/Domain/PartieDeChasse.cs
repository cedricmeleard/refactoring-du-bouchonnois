using System.Collections.Immutable;
using Bouchonnois.Domain.Exceptions;
using static Bouchonnois.Domain.PartieStatus;

namespace Bouchonnois.Domain;

public class PartieDeChasse
{
    private readonly List<Chasseur> _chasseurs;
    private readonly List<Event> _events;
    private PartieDeChasse(
        Guid id,
        Terrain terrain,
        List<(string nom, int nbBalles)> chasseurs,
        Func<DateTime> timeProvider)
    {
        Id = id;
        Terrain = terrain;
        Status = EnCours;

        _chasseurs = new List<Chasseur>();

        foreach (var chasseur in chasseurs) {
            _chasseurs.Add(new Chasseur(chasseur.nom, chasseur.nbBalles));
        }

        string chasseursToString = string.Join(", ",
            _chasseurs.Select(c => c.Nom + $" ({c.BallesRestantes} balles)")
        );

        _events = new List<Event>();
        _events.Add(new Event(timeProvider(),
            $"La partie de chasse commence à {terrain.Nom} avec {chasseursToString}")
        );
    }

    public Guid Id { get; }
    public Terrain Terrain { get; }
    public PartieStatus Status { get; private set; }

    public IReadOnlyList<Chasseur> Chasseurs => _chasseurs.ToImmutableList();
    public IReadOnlyList<Event> Events => _events.ToImmutableList();

    public static PartieDeChasse Create(
        Func<DateTime> timeProvider,
        (string nom, int nbGalinettes) terrainDeChasse,
        List<(string nom, int nbBalles)> chasseurs)
    {
        if (terrainDeChasse.nbGalinettes <= 0) {
            throw new ImpossibleDeDémarrerUnePartieSansGalinettes();
        }
        if (chasseurs.Count == 0) {
            throw new ImpossibleDeDémarrerUnePartieSansChasseur();
        }
        if (chasseurs.Exists(c => c.nbBalles == 0)) {
            throw new ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalle();
        }

        return new PartieDeChasse(
            Guid.NewGuid(),
            new Terrain(terrainDeChasse.nom, terrainDeChasse.nbGalinettes),
            chasseurs,
            timeProvider
        );
    }

    public void StartApero(Func<DateTime> timeProvider)
    {
        if (Status == Apéro) {
            throw new OnEstDéjàEnTrainDePrendreLapéro();
        }
        if (Status == Terminée) {
            throw new OnPrendPasLapéroQuandLaPartieEstTerminée();
        }
        Status = Apéro;
        _events.Add(new Event(timeProvider(), "Petit apéro"));
    }

    public void Reprendre(Func<DateTime> timeProvider)
    {
        if (Status == EnCours) {
            throw new LaChasseEstDéjàEnCours();
        }

        if (Status == Terminée) {
            throw new QuandCestFiniCestFini();
        }

        Status = EnCours;
        _events.Add(new Event(timeProvider(), "Reprise de la chasse"));
    }

    public string Terminer(Func<DateTime> timeProvider)
    {
        var classement = Chasseurs
            .GroupBy(c => c.NbGalinettes)
            .OrderByDescending(g => g.Key);

        if (Status == Terminée) {
            throw new QuandCestFiniCestFini();
        }

        Status = Terminée;

        string result;

        if (classement.All(group => group.Key == 0)) {
            result = "Brocouille";
            _events.Add(
                new Event(timeProvider(), "La partie de chasse est terminée, vainqueur : Brocouille")
            );
        } else {
            result = string.Join(", ", classement.First().Select(c => c.Nom));
            _events.Add(
                new Event(timeProvider(),
                    $"La partie de chasse est terminée, vainqueur : {string.Join(", ", classement.First().Select(c => $"{c.Nom} - {c.NbGalinettes} galinettes"))}"
                )
            );
        }
        return result;
    }
    public void Tirer(string chasseur, Func<DateTime> timeProvider, IPartieDeChasseRepository partieDeChasseRepository)
    {
        Tirer(chasseur,
            $"{chasseur} tire",
            timeProvider,
            partieDeChasseRepository,
            c => { EmitEvent(timeProvider, $"{chasseur} tire"); });
    }

    public void TirerSurUneGalinette(string chasseur, Func<DateTime> timeProvider, IPartieDeChasseRepository partieDeChasseRepository)
    {
        if (Terrain.NbGalinettes == 0) {
            throw new TasTropPicoléMonVieuxTasRienTouché();
        }

        Tirer(chasseur,
            $"{chasseur} veut tirer sur une galinette",
            timeProvider,
            partieDeChasseRepository,
            c =>
            {
                EmitEvent(timeProvider, $"{chasseur} tire sur une galinette");
                c.ATuéUneGalinette();
                Terrain.UneGalinetteEnMoins();
            });
    }

    private void Tirer(
        string chasseur,
        string debutMessage,
        Func<DateTime> timeProvider,
        IPartieDeChasseRepository partieDeChasseRepository,
        Action<Chasseur>? continueWith)
    {
        if (DuringApéro()) {
            EmitEventAndSave(timeProvider, partieDeChasseRepository, $"{chasseur} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!");
            throw new OnTirePasPendantLapéroCestSacré();
        }
        if (DéjaTerminée()) {
            EmitEventAndSave(timeProvider, partieDeChasseRepository, $"{chasseur} veut tirer -> On tire pas quand la partie est terminée");
            throw new OnTirePasQuandLaPartieEstTerminée();
        }

        if (!ChasseurExists(chasseur)) {
            throw new ChasseurInconnu(chasseur);
        }

        var chasseurQuiTire = Chasseurs.First(c => c.Nom == chasseur);

        if (!chasseurQuiTire.AEncoreDesBalles()) {
            EmitEventAndSave(timeProvider, partieDeChasseRepository, $"{debutMessage} -> T'as plus de balles mon vieux, chasse à la main");
            throw new TasPlusDeBallesMonVieuxChasseALaMain();
        }
        chasseurQuiTire.ATiré();
        continueWith?.Invoke(chasseurQuiTire);
    }

    public string Consulter()
        => string.Join(Environment.NewLine,
            _events
                .OrderByDescending(@event => @event.Date)
                .Select(@event => @event.ToString())
        );

    private void EmitEventAndSave(
        Func<DateTime> timeProvider,
        IPartieDeChasseRepository partieDeChasseRepository,
        string message)
    {
        EmitEvent(timeProvider, message);
        partieDeChasseRepository.Save(this);
    }
    private void EmitEvent(Func<DateTime> timeProvider, string message) => _events.Add(new Event(timeProvider(), message));

    private bool ChasseurExists(string chasseur) => _chasseurs.Exists(c => c.Nom == chasseur);
    private bool DéjaTerminée() => Status == Terminée;
    private bool DuringApéro() => Status == Apéro;
}