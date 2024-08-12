using Bouchonnois.Domain;
using Bouchonnois.Service.Exceptions;

namespace Bouchonnois.Tests.Unit;

public class TerminerLaPartieDeChasse : PartieDeChasseServiceTest
{
    [Fact]
    public void QuandLaPartieEstEnCoursEt1SeulChasseurGagne()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            .Avec(Dédé, Bernard, Robert.AvecDesGalinettes(2))
        );

        string meilleurChasseur = PartieDeChasseService.TerminerLaPartie(partieDeChasse.Id);

        Repository
            .SavedPartieDeChasse()
            .Should()
            .HaveEmittedEvent(Now, "La partie de chasse est terminée, vainqueur : Robert - 2 galinettes")
            .And.LaPartieEstTerminée()
            .And.ChasseurATiréSurUneGalinette(Data.Dédé, 20, 0)
            .And.ChasseurATiréSurUneGalinette(Data.Bernard, 8, 0)
            .And.ChasseurATiréSurUneGalinette(Data.Robert, 12, 2);

        meilleurChasseur.Should().Be(Data.Robert);
    }

    [Fact]
    public void QuandLaPartieEstEnCoursEt1SeulChasseurDansLaPartie()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            .Avec(Robert.AvecDesGalinettes(2))
        );

        string meilleurChasseur = PartieDeChasseService.TerminerLaPartie(partieDeChasse.Id);

        Repository
            .SavedPartieDeChasse()
            .Should()
            .HaveEmittedEvent(Now, "La partie de chasse est terminée, vainqueur : Robert - 2 galinettes")
            .And.LaPartieEstTerminée()
            .And.ChasseurATiréSurUneGalinette(Data.Robert, 12, 2);

        meilleurChasseur.Should().Be(Data.Robert);
    }

    [Fact]
    public void QuandLaPartieEstEnCoursEt2ChasseursExAequo()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            // Attention, terrain avec 3 galinette mais 4 chassées, il manque une regle métier ?
            .Avec(Dédé.AvecDesGalinettes(2), Bernard.AvecDesGalinettes(2), Robert)
        );

        string meilleurChasseur = PartieDeChasseService.TerminerLaPartie(partieDeChasse.Id);
        meilleurChasseur.Should().Be($"{Data.Dédé}, {Data.Bernard}");

        Repository
            .SavedPartieDeChasse()
            .Should()
            .HaveEmittedEvent(Now, "La partie de chasse est terminée, vainqueur : Dédé - 2 galinettes, Bernard - 2 galinettes")
            .And.LaPartieEstTerminée()
            .And.ChasseurATiréSurUneGalinette(Data.Dédé, 20, 2)
            .And.ChasseurATiréSurUneGalinette(Data.Bernard, 8, 2)
            .And.ChasseurATiréSurUneGalinette(Data.Robert, 12, 0);
    }

    [Fact]
    public void QuandLaPartieEstEnCoursEtToutLeMondeBrocouille()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            // Attention, terrain avec 3 galinette mais 4 chassées, il manque une regle métier ?
            .Avec(Dédé, Bernard, Robert)
        );

        string meilleurChasseur = PartieDeChasseService.TerminerLaPartie(partieDeChasse.Id);
        meilleurChasseur.Should().Be("Brocouille");

        Repository
            .SavedPartieDeChasse()
            .Should()
            .HaveEmittedEvent(Now, "La partie de chasse est terminée, vainqueur : Brocouille")
            .And.LaPartieEstTerminée()
            .And.ChasseurATiréSurUneGalinette(Data.Dédé, 20, 0)
            .And.ChasseurATiréSurUneGalinette(Data.Bernard, 8, 0)
            .And.ChasseurATiréSurUneGalinette(Data.Robert, 12, 0);
    }

    [Fact]
    public void QuandLesChasseursSontALaperoEtTousExAequo()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(
            NouvellePartieDeChasse
                .AvecUnTerrainRicheEnGalinettes(3)
            // Attention, terrain avec 3 galinette mais 4 chassées, il manque une regle métier ?
                .Avec(Dédé.AvecDesGalinettes(3), Bernard.AvecDesGalinettes(3), Robert.AvecDesGalinettes(3))
                .AlorsQueLaPartieEst(PartieStatus.Apéro)
        );

        string meilleurChasseur = PartieDeChasseService.TerminerLaPartie(partieDeChasse.Id);

        Repository
            .SavedPartieDeChasse()
            .Should()
            .HaveEmittedEvent(Now, "La partie de chasse est terminée, vainqueur : Dédé - 3 galinettes, Bernard - 3 galinettes, Robert - 3 galinettes")
            .And.LaPartieEstTerminée()
            .And.ChasseurATiréSurUneGalinette(Data.Dédé, 20, 3)
            .And.ChasseurATiréSurUneGalinette(Data.Bernard, 8, 3)
            .And.ChasseurATiréSurUneGalinette(Data.Robert, 12, 3);

        meilleurChasseur.Should().Be($"{Data.Dédé}, {Data.Bernard}, {Data.Robert}");
    }

    public class Failure : PartieDeChasseServiceTest
    {
        [Fact]
        public void EchoueSiLaPartieDeChasseEstDéjàTerminée()
        {
            var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
                .AvecUnTerrainRicheEnGalinettes(3)
                // Attention, terrain avec 3 galinette mais 4 chassées, il manque une regle métier ?
                .Avec(Dédé, Bernard, Robert.AvecDesGalinettes(2))
                .AlorsQueLaPartieEst(PartieStatus.Terminée)
            );

            var prendreLapéroQuandTerminée = () => PartieDeChasseService.TerminerLaPartie(partieDeChasse.Id);

            prendreLapéroQuandTerminée.Should()
                .Throw<QuandCestFiniCestFini>();

            Repository.SavedPartieDeChasse().Should().BeNull();
        }
    }
}