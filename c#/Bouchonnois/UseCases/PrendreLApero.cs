using Bouchonnois.Domain;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.UseCases;

public class PrendreLApero(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public void Handle(Guid id)
    {
        var partieDeChasse = repository.GetById(id);

        if (partieDeChasse == null) {
            throw new LaPartieDeChasseNexistePas();
        }

        partieDeChasse.StartApero(timeProvider);
        repository.Save(partieDeChasse);
    }
}