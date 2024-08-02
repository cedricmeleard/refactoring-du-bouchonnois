using Bouchonnois.Domain;
using Bouchonnois.Service.Exceptions;

namespace Bouchonnois.Tests.Unit;

public class ReprendreLaPartieDeChasse : PartieDeChasseServiceTest
{
    [Fact]
    public void QuandLapéroEstEnCours()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            .Avec(Dédé, Bernard, Robert)
            .AlorsQueLaPartieEst(PartieStatus.Apéro)
        );

        PartieDeChasseService.ReprendreLaPartie(partieDeChasse.Id);

        Repository
            .SavedPartieDeChasse()
            .Should()
            .HaveEmittedEvent(Now, "Reprise de la chasse")
            .And.GalinettesSurLeTerrain(3)
            .And.LaPartieEstEnCours()
            .And.ChasseurATiréSurUneGalinette("Dédé", 20, 0)
            .And.ChasseurATiréSurUneGalinette("Bernard", 8, 0)
            .And.ChasseurATiréSurUneGalinette("Robert", 12, 0);
    }

    public class Failure : PartieDeChasseServiceTest
    {
        [Fact]
        public void EchoueCarPartieNexistePas()
        {
            var id = Guid.NewGuid();
            var reprendrePartieQuandPartieExistePas = () => PartieDeChasseService.ReprendreLaPartie(id);

            reprendrePartieQuandPartieExistePas.Should()
                .Throw<LaPartieDeChasseNexistePas>();
            Repository.SavedPartieDeChasse().Should().BeNull();
        }

        [Fact]
        public void EchoueSiLaChasseEstEnCours()
        {
            var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
                .AvecUnTerrainRicheEnGalinettes(3)
                .Avec(Dédé, Bernard, Robert)
            );

            var reprendreLaPartieQuandChasseEnCours = () => PartieDeChasseService.ReprendreLaPartie(partieDeChasse.Id);

            reprendreLaPartieQuandChasseEnCours.Should()
                .Throw<LaChasseEstDéjàEnCours>();

            Repository.SavedPartieDeChasse().Should().BeNull();
        }

        [Fact]
        public void EchoueSiLaPartieDeChasseEstTerminée()
        {
            var partieDeChasse = AvecUnePartieDeChasseExistante(
                NouvellePartieDeChasse
                    .AvecUnTerrainRicheEnGalinettes(3)
                    .Avec(Dédé, Bernard, Robert)
                    .AlorsQueLaPartieEst(PartieStatus.Terminée)
            );

            var prendreLapéroQuandTerminée = () => PartieDeChasseService.ReprendreLaPartie(partieDeChasse.Id);

            prendreLapéroQuandTerminée.Should()
                .Throw<QuandCestFiniCestFini>();

            Repository.SavedPartieDeChasse().Should().BeNull();
        }
    }
}