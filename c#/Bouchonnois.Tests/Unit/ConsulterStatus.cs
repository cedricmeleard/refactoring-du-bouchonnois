using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.Tests.Unit;

public class ConsulterStatus : UseCaseTest<UseCases.ConsulterStatus>
{
    public ConsulterStatus() : base((r, t) => new UseCases.ConsulterStatus(r))
    {
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = Guid.NewGuid();
        var reprendrePartieQuandPartieExistePas = () => UseCase.Handle(id);

        reprendrePartieQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}