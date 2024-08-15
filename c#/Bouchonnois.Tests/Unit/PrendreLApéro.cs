using Bouchonnois.Domain;
using Bouchonnois.Domain.Exceptions;
using Bouchonnois.UseCases;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.Tests.Unit;

public class PrendreLApéro : UseCaseTest<PrendreLApero>
{
    public PrendreLApéro() : base((r, t) => new PrendreLApero(r, t)) {}

    [Fact]
    public void QuandLaPartieEstEnCours()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(
            NouvellePartieDeChasse
                .AvecUnTerrainRicheEnGalinettes(3)
                .Avec(Dédé, Bernard, Robert)
        );
        UseCase.Handle(partieDeChasse.Id);

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
        var apéroQuandPartieExistePas = () => UseCase.Handle(id);

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

        var prendreLApéroQuandOnPrendDéjàLapéro = () => UseCase.Handle(partieDeChasse.Id);

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

        var prendreLapéroQuandTerminée = () => UseCase.Handle(partieDeChasse.Id);

        prendreLapéroQuandTerminée.Should()
            .Throw<OnPrendPasLapéroQuandLaPartieEstTerminée>();
        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}