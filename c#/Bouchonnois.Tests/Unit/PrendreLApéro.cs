using Bouchonnois.Domain;
using Bouchonnois.Service.Exceptions;

namespace Bouchonnois.Tests.Unit;

public class PrendreLApéro : PartieDeChasseServiceTest
{
    [Fact]
    public void QuandLaPartieEstEnCours()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(
            NouvellePartieDeChasse
                .AvecUnTerrainRicheEnGalinettes(3)
                .Avec(Dédé, Bernard, Robert)
        );
        PartieDeChasseService.PrendreLapéro(partieDeChasse.Id);

        Repository
            .SavedPartieDeChasse()
            .Should()
            .HaveEmittedEvent(Now, "Petit apéro")
            .And.CestLePetitApero();
    }

    public class Failure : PartieDeChasseServiceTest
    {
        [Fact]
        public void EchoueCarPartieNexistePas()
        {
            var id = Guid.NewGuid();
            var apéroQuandPartieExistePas = () => PartieDeChasseService.PrendreLapéro(id);

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

            var prendreLApéroQuandOnPrendDéjàLapéro = () => PartieDeChasseService.PrendreLapéro(partieDeChasse.Id);

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

            var prendreLapéroQuandTerminée = () => PartieDeChasseService.PrendreLapéro(partieDeChasse.Id);

            prendreLapéroQuandTerminée.Should()
                .Throw<OnPrendPasLapéroQuandLaPartieEstTerminée>();
            Repository.SavedPartieDeChasse().Should().BeNull();
        }
    }
}