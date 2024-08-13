using Bouchonnois.UseCases.Exceptions;

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
    public Guid Id { get; init; }
    public Terrain Terrain { get; }
    public PartieStatus Status { get; set; }
    public List<Chasseur> Chasseurs { get; init; }
    public List<Event> Events { get; init; }
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
}