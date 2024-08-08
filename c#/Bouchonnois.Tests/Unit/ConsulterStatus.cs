using Bouchonnois.Domain;
using Bouchonnois.Service.Exceptions;

namespace Bouchonnois.Tests.Unit;

public class ConsulterStatus : PartieDeChasseServiceTest
{
    [Fact]
    public void QuandLaPartieVientDeDémarrer()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(
            NouvellePartieDeChasse
                .AvecUnTerrainRicheEnGalinettes(3)
                .Avec(Dédé, Bernard, Robert.AvecDesGalinettes(2))
                .AlorsQueLaPartieEst(PartieStatus.Terminée)
                .WithAnEventLog(new DateTime(2024, 4, 25, 9, 0, 12),
                "La partie de chasse commence à Pitibon sur Sauldre avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)")
        );

        string status = PartieDeChasseService.ConsulterStatus(partieDeChasse.Id);

        status.Should()
            .Be(
                "09:00 - La partie de chasse commence à Pitibon sur Sauldre avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)"
            );
    }

    [Fact]
    public Task QuandLaPartieEstTerminée()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(
            NouvellePartieDeChasse
                .AvecUnTerrainRicheEnGalinettes(3)
                .Avec(Dédé(), Bernard(), Robert().AvecDesGalinettes(2))
                .AlorsQueLaPartieEst(PartieStatus.Terminée)
                .WithEventLogs(
                (new DateTime(2024, 4, 25, 9, 0, 12),
                    "La partie de chasse commence à Pitibon sur Sauldre avec Dédé (20 balles), Bernard (8 balles), Robert (12 balles)"),
                (new DateTime(2024, 4, 25, 9, 10, 0), "Dédé tire"),
                (new DateTime(2024, 4, 25, 9, 40, 0), "Robert tire sur une galinette"),
                (new DateTime(2024, 4, 25, 10, 0, 0), "Petit apéro"),
                (new DateTime(2024, 4, 25, 11, 0, 0), "Reprise de la chasse"),
                (new DateTime(2024, 4, 25, 11, 2, 0), "Bernard tire"),
                (new DateTime(2024, 4, 25, 11, 3, 0), "Bernard tire"),
                (new DateTime(2024, 4, 25, 11, 4, 0), "Dédé tire sur une galinette"),
                (new DateTime(2024, 4, 25, 11, 30, 0), "Robert tire sur une galinette"),
                (new DateTime(2024, 4, 25, 11, 40, 0), "Petit apéro"),
                (new DateTime(2024, 4, 25, 14, 30, 0), "Reprise de la chasse"),
                (new DateTime(2024, 4, 25, 14, 41, 0), "Bernard tire"),
                (new DateTime(2024, 4, 25, 14, 41, 1), "Bernard tire"),
                (new DateTime(2024, 4, 25, 14, 41, 2), "Bernard tire"),
                (new DateTime(2024, 4, 25, 14, 41, 3), "Bernard tire"),
                (new DateTime(2024, 4, 25, 14, 41, 4), "Bernard tire"),
                (new DateTime(2024, 4, 25, 14, 41, 5), "Bernard tire"),
                (new DateTime(2024, 4, 25, 14, 41, 6), "Bernard tire"),
                (new DateTime(2024, 4, 25, 14, 41, 7), "Bernard tire -> T'as plus de balles mon vieux, chasse à la main"),
                (new DateTime(2024, 4, 25, 15, 0, 0), "Robert tire sur une galinette"),
                (new DateTime(2024, 4, 25, 15, 30, 0), "La partie de chasse est terminée, vainqueur :  Robert - 3 galinettes"))
        );

        return Verify(PartieDeChasseService.ConsulterStatus(partieDeChasse.Id));
    }

    public class Failure : PartieDeChasseServiceTest
    {
        [Fact]
        public void EchoueCarPartieNexistePas()
        {
            var id = Guid.NewGuid();
            var reprendrePartieQuandPartieExistePas = () => PartieDeChasseService.ConsulterStatus(id);

            reprendrePartieQuandPartieExistePas.Should()
                .Throw<LaPartieDeChasseNexistePas>();
            Repository.SavedPartieDeChasse().Should().BeNull();
        }
    }
}