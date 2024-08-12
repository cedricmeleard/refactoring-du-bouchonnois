using Bouchonnois.Domain;
using Bouchonnois.UseCases;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.Tests.Unit;

public class PrendreLApéro : PartieDeChasseServiceTest
{
    private readonly PrendreLApero _useCase;
    public PrendreLApéro()
    {
        _useCase = new PrendreLApero(Repository, TimeProvider);
    }

    [Fact]
    public void QuandLaPartieEstEnCours()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(
            NouvellePartieDeChasse
                .AvecUnTerrainRicheEnGalinettes(3)
                .Avec(Dédé, Bernard, Robert)
        );
        _useCase.Handle(partieDeChasse.Id);

        Repository
            .SavedPartieDeChasse()
            .Should()
            .HaveEmittedEvent(Now, "Petit apéro")
            .And.CestLePetitApero();
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = Guid.NewGuid();
        var apéroQuandPartieExistePas = () => _useCase.Handle(id);

        apéroQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontDéjaEnApero()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            .Avec(Dédé, Bernard, Robert)
            .AlorsQueLaPartieEst(PartieStatus.Apéro)
        );

        var prendreLApéroQuandOnPrendDéjàLapéro = () => _useCase.Handle(partieDeChasse.Id);

        prendreLApéroQuandOnPrendDéjàLapéro.Should()
            .Throw<OnEstDéjàEnTrainDePrendreLapéro>();
        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            .Avec(Dédé, Bernard, Robert)
            .AlorsQueLaPartieEst(PartieStatus.Terminée)
        );

        var prendreLapéroQuandTerminée = () => _useCase.Handle(partieDeChasse.Id);

        prendreLapéroQuandTerminée.Should()
            .Throw<OnPrendPasLapéroQuandLaPartieEstTerminée>();
        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}