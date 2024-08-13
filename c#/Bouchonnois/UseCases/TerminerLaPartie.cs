using Bouchonnois.Domain;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.UseCases;

public class TerminerLaPartie(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
{
    public string Handle(Guid id)
    {
        var partieDeChasse = repository.GetById(id);

        if (partieDeChasse == null) {
            throw new LaPartieDeChasseNexistePas();
        }

        string result = partieDeChasse.Terminer(timeProvider);

        repository.Save(partieDeChasse);

        return result;
    }
}