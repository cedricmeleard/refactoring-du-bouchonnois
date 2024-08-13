using Bouchonnois.Domain.Exceptions;

namespace Bouchonnois.Domain;

public class PartieDeChasse
{
    public PartieDeChasse(Guid id, Terrain terrain)
    {
        Id = id;
        Terrain = terrain;
        Status = PartieStatus.EnCours;
        Chasseurs = new List<Chasseur>();
        Events = new List<Event>();
    }
    public Guid Id { get; }
    public Terrain Terrain { get; }
    public PartieStatus Status { get; set; }
    public List<Chasseur> Chasseurs { get; init; }
    public List<Event> Events { get; init; }

    public void StartApero(Func<DateTime> timeProvider)
    {
        if (Status == PartieStatus.Apéro) {
            throw new OnEstDéjàEnTrainDePrendreLapéro();
        }
        if (Status == PartieStatus.Terminée) {
            throw new OnPrendPasLapéroQuandLaPartieEstTerminée();
        }
        Status = PartieStatus.Apéro;
        Events.Add(new Event(timeProvider(), "Petit apéro"));
    }

    public void Reprendre(Func<DateTime> timeProvider)
    {
        if (Status == PartieStatus.EnCours) {
            throw new LaChasseEstDéjàEnCours();
        }

        if (Status == PartieStatus.Terminée) {
            throw new QuandCestFiniCestFini();
        }

        Status = PartieStatus.EnCours;
        Events.Add(new Event(timeProvider(), "Reprise de la chasse"));
    }

    public string Terminer(Func<DateTime> timeProvider)
    {
        var classement = Chasseurs
            .GroupBy(c => c.NbGalinettes)
            .OrderByDescending(g => g.Key);

        if (Status == PartieStatus.Terminée) {
            throw new QuandCestFiniCestFini();
        }

        Status = PartieStatus.Terminée;

        string result;

        if (classement.All(group => group.Key == 0)) {
            result = "Brocouille";
            Events.Add(
                new Event(timeProvider(), "La partie de chasse est terminée, vainqueur : Brocouille")
            );
        } else {
            result = string.Join(", ", classement.First().Select(c => c.Nom));
            Events.Add(
                new Event(timeProvider(),
                    $"La partie de chasse est terminée, vainqueur : {string.Join(", ", classement.First().Select(c => $"{c.Nom} - {c.NbGalinettes} galinettes"))}"
                )
            );
        }
        return result;
    }
    public void Tirer(string chasseur, Func<DateTime> timeProvider, IPartieDeChasseRepository partieDeChasseRepository)
    {
        if (Status != PartieStatus.Apéro) {
            if (Status != PartieStatus.Terminée) {
                if (Chasseurs.Exists(c => c.Nom == chasseur)) {
                    var chasseurQuiTire = Chasseurs.First(c => c.Nom == chasseur);

                    if (chasseurQuiTire.BallesRestantes == 0) {
                        Events.Add(new Event(timeProvider(),
                            $"{chasseur} tire -> T'as plus de balles mon vieux, chasse à la main"));
                        partieDeChasseRepository.Save(this);

                        throw new TasPlusDeBallesMonVieuxChasseALaMain();
                    }

                    Events.Add(new Event(timeProvider(), $"{chasseur} tire"));
                    chasseurQuiTire.BallesRestantes--;
                } else {
                    throw new ChasseurInconnu(chasseur);
                }
            } else {
                Events.Add(new Event(timeProvider(),
                    $"{chasseur} veut tirer -> On tire pas quand la partie est terminée"));
                partieDeChasseRepository.Save(this);

                throw new OnTirePasQuandLaPartieEstTerminée();
            }
        } else {
            Events.Add(new Event(timeProvider(),
                $"{chasseur} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!"));
            partieDeChasseRepository.Save(this);

            throw new OnTirePasPendantLapéroCestSacré();
        }
    }
    public void TirerSurUneGalinette(string chasseur, Func<DateTime> timeProvider, IPartieDeChasseRepository partieDeChasseRepository)
    {
        if (Terrain.NbGalinettes != 0) {
            if (Status != PartieStatus.Apéro) {
                if (Status != PartieStatus.Terminée) {
                    if (Chasseurs.Exists(c => c.Nom == chasseur)) {
                        var chasseurQuiTire = Chasseurs.First(c => c.Nom == chasseur);

                        if (chasseurQuiTire.BallesRestantes == 0) {
                            Events.Add(new Event(timeProvider(),
                                $"{chasseur} veut tirer sur une galinette -> T'as plus de balles mon vieux, chasse à la main"));
                            partieDeChasseRepository.Save(this);

                            throw new TasPlusDeBallesMonVieuxChasseALaMain();
                        }

                        chasseurQuiTire.BallesRestantes--;
                        chasseurQuiTire.NbGalinettes++;
                        Terrain.NbGalinettes--;
                        Events.Add(new Event(timeProvider(), $"{chasseur} tire sur une galinette"));
                    } else {
                        throw new ChasseurInconnu(chasseur);
                    }
                } else {
                    Events.Add(new Event(timeProvider(),
                        $"{chasseur} veut tirer -> On tire pas quand la partie est terminée"));
                    partieDeChasseRepository.Save(this);

                    throw new OnTirePasQuandLaPartieEstTerminée();
                }
            } else {
                Events.Add(new Event(timeProvider(),
                    $"{chasseur} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!"));
                partieDeChasseRepository.Save(this);
                throw new OnTirePasPendantLapéroCestSacré();
            }
        } else {
            throw new TasTropPicoléMonVieuxTasRienTouché();
        }
    }
    public string Consulter()
    {
        return string.Join(
            Environment.NewLine,
            Events
                .OrderByDescending(@event => @event.Date)
                .Select(@event => @event.ToString())
        );
    }
}