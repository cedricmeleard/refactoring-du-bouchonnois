using Bouchonnois.Service.Exceptions;

namespace Bouchonnois.Tests.Unit;

public class DemarrerUnePartieDeChasse : PartieDeChasseServiceTest
{
    [Fact]
    public void AvecPlusieursChasseurs()
    {
        var chasseurs = new List<(string, int)>
        {
            ("Dédé", 20),
            ("Bernard", 8),
            ("Robert", 12)
        };
        var terrainDeChasse = (terrainName: TestConstants.TerrainName, 3);

        PartieDeChasseService.Demarrer(
            terrainDeChasse,
            chasseurs
        );

        Repository
            .SavedPartieDeChasse()
            .Should()
            .HaveEmittedEvent(Now,
                $"La partie de chasse commence à {TestConstants.TerrainName} avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)")
            .And.ChasseurATiréSurUneGalinette("Dédé", 20, 0)
            .And.ChasseurATiréSurUneGalinette("Bernard", 8, 0)
            .And.ChasseurATiréSurUneGalinette("Robert", 12, 0)
            .And.GalinettesSurLeTerrain(3)
            .And.LaPartieEstEnCours();
    }

    public class Failure : PartieDeChasseServiceTest
    {
        [Fact]
        public void EchoueSansChasseurs()
        {
            var chasseurs = new List<(string, int)>();
            var terrainDeChasse = (terrainName: TestConstants.TerrainName, 3);

            Action demarrerPartieSansChasseurs = () => PartieDeChasseService.Demarrer(terrainDeChasse, chasseurs);

            demarrerPartieSansChasseurs.Should()
                .Throw<ImpossibleDeDémarrerUnePartieSansChasseur>();
            Repository.SavedPartieDeChasse().Should().BeNull();
        }

        [Fact]
        public void EchoueAvecUnTerrainSansGalinettes()
        {
            var chasseurs = new List<(string, int)>();
            var terrainDeChasse = (terrainName: TestConstants.TerrainName, 0);

            Action demarrerPartieSansChasseurs = () => PartieDeChasseService.Demarrer(terrainDeChasse, chasseurs);

            demarrerPartieSansChasseurs.Should()
                .Throw<ImpossibleDeDémarrerUnePartieSansGalinettes>();
        }

        [Fact]
        public void EchoueSiChasseurSansBalle()
        {
            var chasseurs = new List<(string, int)>
            {
                ("Dédé", 20),
                ("Bernard", 0)
            };
            var terrainDeChasse = (terrainName: TestConstants.TerrainName, 3);

            Action demarrerPartieAvecChasseurSansBalle = () => PartieDeChasseService.Demarrer(terrainDeChasse, chasseurs);

            demarrerPartieAvecChasseurSansBalle.Should()
                .Throw<ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalle>();
            Repository.SavedPartieDeChasse().Should().BeNull();
        }
    }
}