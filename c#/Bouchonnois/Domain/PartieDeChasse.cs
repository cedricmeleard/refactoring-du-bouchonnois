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
}