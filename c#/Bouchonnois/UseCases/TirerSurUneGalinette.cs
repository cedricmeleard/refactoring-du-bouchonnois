using Bouchonnois.Domain;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.UseCases;

public class TirerSurUneGalinette(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public void Handle(Guid id, string chasseur)
    {
        var partieDeChasse = repository.GetById(id);

        if (partieDeChasse == null) {
            throw new LaPartieDeChasseNexistePas();
        }

        partieDeChasse.TirerSurUneGalinette(chasseur, timeProvider, repository);

        repository.Save(partieDeChasse);
    }
}