using Bouchonnois.Domain;
using Bouchonnois.Domain.Exceptions;

namespace Bouchonnois.UseCases;

public class DemarrerUnePartieDeChasse(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public Guid Handle((string nom, int nbGalinettes) terrainDeChasse, List<(string nom, int nbBalles)> chasseurs)
    {
        if (terrainDeChasse.nbGalinettes <= 0) {
            throw new ImpossibleDeDémarrerUnePartieSansGalinettes();
        }

        var partieDeChasse = PartieDeChasse.Create(timeProvider, terrainDeChasse, chasseurs);

        repository.Save(partieDeChasse);

        return partieDeChasse.Id;
    }
}