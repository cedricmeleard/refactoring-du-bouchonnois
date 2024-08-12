using Bouchonnois.Domain;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.UseCases;

public class ConsulterStatusUseCase
{
    private readonly IPartieDeChasseRepository _repository;
    public ConsulterStatusUseCase(IPartieDeChasseRepository repository)
    {
        _repository = repository;
    }
    public string ConsulterStatus(Guid id)
    {
        var partieDeChasse = _repository.GetById(id);

        if (partieDeChasse == null) {
            throw new LaPartieDeChasseNexistePas();
        }

        return string.Join(
            Environment.NewLine,
            partieDeChasse.Events
                .OrderByDescending(@event => @event.Date)
                .Select(@event => @event.ToString())
        );
    }
}