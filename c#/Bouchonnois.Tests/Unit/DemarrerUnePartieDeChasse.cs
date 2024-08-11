using Bouchonnois.Domain;
using Bouchonnois.Service.Exceptions;
using FsCheck;
using FsCheck.Xunit;
using Microsoft.FSharp.Collections;

namespace Bouchonnois.Tests.Unit;

public class DemarrerUnePartieDeChasse : PartieDeChasseServiceTest
{
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

    [Property]
    public Property Sur1TerrainAvecGalinettesEtAuMoins1ChasseurAvecTousDesBalles()
        => Prop.ForAll(
            TerrainAvecGalinettesGenerator(),
            GroupeDeChasseursAvecBallesGenerator(),
            (terrain, chasseurs) => DémarreLaPartieAvecSuccès(terrain, chasseurs)
        );
    private bool DémarreLaPartieAvecSuccès((string nom, int nbGalinettes) terrain, FSharpList<(string nom, int nbBalles)> chasseurs)
        => PartieDeChasseService.Demarrer(
            terrain,
            chasseurs.ToList()) == Repository.SavedPartieDeChasse()!.Id;


    public class Failure : PartieDeChasseServiceTest
    {
        [Property]
        public Property SansChasseur()
            => Prop.ForAll(
                TerrainAvecGalinettesGenerator(),
                terrain =>
                    EchoueAvec<ImpossibleDeDémarrerUnePartieSansChasseur>(
                        terrain,
                        PasDeChasseurs,
                        savedPartieDeChasse => savedPartieDeChasse == null));

        [Property]
        public Property TerrainSansGalinette()
            => Prop.ForAll(
                TerrainSansGalinetteGenerator(),
                GroupeDeChasseursAvecBallesGenerator(),
                (terrain, chasseurs) =>
                    EchoueAvec<ImpossibleDeDémarrerUnePartieSansGalinettes>(
                        terrain,
                        chasseurs,
                        savedPartieDeChasse => savedPartieDeChasse == null));

        [Property]
        public Property ChasseurSansBalle()
            => Prop.ForAll(
                TerrainAvecGalinettesGenerator(),
                GroupeDeChasseursSansBalleGenerator(),
                (terrain, chasseurs) =>
                    EchoueAvec<ImpossibleDeDémarrerUnePartieAvecUnChasseurSansBalle>(
                        terrain,
                        chasseurs,
                        savedPartieDeChasse => savedPartieDeChasse == null));

        private bool EchoueAvec<TException>(
            (string nom, int nbGalinettes) terrain,
            IEnumerable<(string nom, int nbBalles)> chasseurs,
            Func<PartieDeChasse?, bool>? assert = null) where TException : Exception
            => MustFailWith<TException>(() => PartieDeChasseService.Demarrer(terrain, chasseurs.ToList()), assert);
    }
}