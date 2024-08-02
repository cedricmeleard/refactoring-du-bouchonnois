using Bouchonnois.Domain;
using Bouchonnois.Service.Exceptions;

namespace Bouchonnois.Tests.Unit;

public class Tirer : PartieDeChasseServiceTest
{
    [Fact]
    public void AvecUnChasseurAyantDesBalles()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            .Avec(Dédé, Bernard, Robert)
        );

        PartieDeChasseService.Tirer(partieDeChasse.Id, "Bernard");

        Repository
            .SavedPartieDeChasse()
            .Should()
            .HaveEmittedEvent(Now, "Bernard tire")
            .And.LaPartieEstEnCours()
            .And.ChasseurATiréSurUneGalinette("Dédé", 20, 0)
            .And.ChasseurATiréSurUneGalinette("Bernard", 7, 0)
            .And.ChasseurATiréSurUneGalinette("Robert", 12, 0)
            .And.GalinettesSurLeTerrain(3);
    }

    public class Failure : PartieDeChasseServiceTest
    {
        [Fact]
        public void EchoueCarPartieNexistePas()
        {
            var id = Guid.NewGuid();
            var tirerQuandPartieExistePas = () => PartieDeChasseService.Tirer(id, "Bernard");

            tirerQuandPartieExistePas.Should()
                .Throw<LaPartieDeChasseNexistePas>();
            Repository.SavedPartieDeChasse().Should().BeNull();
        }

        [Fact]
        public void EchoueAvecUnChasseurNayantPlusDeBalles()
        {
            var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
                .AvecUnTerrainRicheEnGalinettes(3)
                .Avec(Dédé, Bernard.AvecDesBallesRestantes(0), Robert)
            );

            var tirerSansBalle = () => PartieDeChasseService.Tirer(partieDeChasse.Id, "Bernard");

            tirerSansBalle.Should()
                .Throw<TasPlusDeBallesMonVieuxChasseALaMain>();

            AssertLastEvent(partieDeChasse, "Bernard tire -> T'as plus de balles mon vieux, chasse à la main");
        }

        [Fact]
        public void EchoueCarLeChasseurNestPasDansLaPartie()
        {
            var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
                .AvecUnTerrainRicheEnGalinettes(3)
                .Avec(Dédé, Bernard, Robert)
            );

            var chasseurInconnuVeutTirer = () => PartieDeChasseService.Tirer(partieDeChasse.Id, "Chasseur inconnu");

            chasseurInconnuVeutTirer.Should()
                .Throw<ChasseurInconnu>();
            Repository.SavedPartieDeChasse().Should().BeNull();
        }

        [Theory]
        [InlineData("Bernard")]
        [InlineData("Michel")]
        [InlineData("Chasseur inconnu")]
        public void EchoueSiLesChasseursSontEnApero(string name)
        {
            var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
                .AvecUnTerrainRicheEnGalinettes(3)
                .Avec(Dédé, Bernard, Robert)
                .AlorsQueLaPartieEst(PartieStatus.Apéro)
            );

            var tirerEnPleinApéro = () => PartieDeChasseService.Tirer(partieDeChasse.Id, name);

            tirerEnPleinApéro.Should()
                .Throw<OnTirePasPendantLapéroCestSacré>();

            AssertLastEvent(partieDeChasse, $"{name} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!");
        }

        [Theory]
        [InlineData("Bernard")]
        [InlineData("Michel")]
        [InlineData("Chasseur inconnu")]
        public void EchoueSiLaPartieDeChasseEstTerminée(string name)
        {
            var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
                .AvecUnTerrainRicheEnGalinettes(3)
                .Avec(Dédé, Bernard, Robert)
                .AlorsQueLaPartieEst(PartieStatus.Terminée)
            );

            var tirerQuandTerminée = () => PartieDeChasseService.Tirer(partieDeChasse.Id, name);

            tirerQuandTerminée.Should()
                .Throw<OnTirePasQuandLaPartieEstTerminée>();

            AssertLastEvent(partieDeChasse, $"{name} veut tirer -> On tire pas quand la partie est terminée");
        }
    }
}