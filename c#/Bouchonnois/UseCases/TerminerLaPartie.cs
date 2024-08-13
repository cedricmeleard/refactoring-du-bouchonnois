using Bouchonnois.Domain;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.UseCases;

public class TerminerLaPartie
{
    private readonly IPartieDeChasseRepository _repository;
    private readonly Func<DateTime> _timeProvider;
    public TerminerLaPartie(IPartieDeChasseRepository repository, Func<DateTime> timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }
    public string Handle(Guid id)
    {
        var partieDeChasse = _repository.GetById(id);

        if (partieDeChasse == null) {
            throw new LaPartieDeChasseNexistePas();
        }

        string result = partieDeChasse.Terminer(_timeProvider);

        _repository.Save(partieDeChasse);

        return result;
    }
}