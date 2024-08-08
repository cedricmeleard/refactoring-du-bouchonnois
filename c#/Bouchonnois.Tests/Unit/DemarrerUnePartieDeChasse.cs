using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Builders;

namespace Bouchonnois.Tests.Unit;

public class DemarrerUnePartieDeChasse : PartieDeChasseServiceTest
{
    private PartieDeChasseCommandBuilder DémarrerUnePartieDeChasse()
    {
        return new PartieDeChasseCommandBuilder();
    }
    
    [Fact]
    public Task AvecPlusieursChasseurs()
    {
        var command = DémarrerUnePartieDeChasse()
            .Avec((Data.Dédé, 20), (Data.Bernard, 8), (Data.Robert, 12))
            .SurUnTerrainRicheEnGalinettes();
        
        PartieDeChasseService.Demarrer(
            command.Terrain,
            command.Chasseurs
        );

        return Verify(Repository
                .SavedPartieDeChasse())
            .DontScrubDateTimes();
    }

    public class Failure : PartieDeChasseServiceTest
    {
        [Fact]
        public void EchoueSansChasseurs()
        {
            var chasseurs = new List<(string, int)>();
            var terrainDeChasse = (terrainName: Data.TerrainName, 3);

            Action demarrerPartieSansChasseurs = () => PartieDeChasseService.Demarrer(terrainDeChasse, chasseurs);

            demarrerPartieSansChasseurs.Should()
                .Throw<ImpossibleDeDémarrerUnePartieSansChasseur>();
            Repository.SavedPartieDeChasse().Should().BeNull();
        }

        [Fact]
        public void EchoueAvecUnTerrainSansGalinettes()
        {
            var chasseurs = new List<(string, int)>();
            var terrainDeChasse = (terrainName: Data.TerrainName, 0);

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
            var terrainDeChasse = (terrainName: Data.TerrainName, 3);

            Action demarrerPartieAvecChasseurSansBalle = () => PartieDeChasseService.Demarrer(terrainDeChasse, chasseurs);

            demarrerPartieAvecChasseurSansBalle.Should()
                .Throw<ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalle>();
            Repository.SavedPartieDeChasse().Should().BeNull();
        }
    }
}