using Bouchonnois.Domain;
using Bouchonnois.Domain.Exceptions;
using Bouchonnois.UseCases;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.Tests.Unit;

public class TerminerLaPartieDeChasse : UseCaseTest<TerminerLaPartie>
{
    public TerminerLaPartieDeChasse() : base((r, t) => new TerminerLaPartie(r, t))
    {
    }

    [Fact]
    public void QuandLaPartieEstEnCoursEt1SeulChasseurGagne()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            .Avec(Dédé, Bernard, Robert.AyantTué(2))
        );

        string meilleurChasseur = UseCase.Handle(partieDeChasse.Id);

        Repository
            .SavedPartieDeChasse()
            .Should()
            .HaveEmittedEvent(Now, "La partie de chasse est terminée, vainqueur : Robert - 2 galinettes")
            .And.LaPartieEstTerminée()
            .And.ChasseurATiréSurUneGalinette(Data.Dédé, 20, 0)
            .And.ChasseurATiréSurUneGalinette(Data.Bernard, 8, 0)
            .And.ChasseurATiréSurUneGalinette(Data.Robert, 10, 2);

        meilleurChasseur.Should().Be(Data.Robert);
    }

    [Fact]
    public void QuandLaPartieEstEnCoursEt1SeulChasseurDansLaPartie()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            .Avec(Robert.AyantTué(2))
        );

        string meilleurChasseur = UseCase.Handle(partieDeChasse.Id);

        Repository
            .SavedPartieDeChasse()
            .Should()
            .HaveEmittedEvent(Now, "La partie de chasse est terminée, vainqueur : Robert - 2 galinettes")
            .And.LaPartieEstTerminée()
            .And.ChasseurATiréSurUneGalinette(Data.Robert, 10, 2);

        meilleurChasseur.Should().Be(Data.Robert);
    }

    [Fact]
    public void QuandLaPartieEstEnCoursEt2ChasseursExAequo()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(4)
            // Attention, terrain avec 3 galinette mais 4 chassées, il manque une regle métier ?
            .Avec(Dédé.AyantTué(2), Bernard.AyantTué(2), Robert)
        );

        string meilleurChasseur = UseCase.Handle(partieDeChasse.Id);
        meilleurChasseur.Should().Be($"{Data.Dédé}, {Data.Bernard}");

        var sut = Repository.SavedPartieDeChasse();

        sut.Should()
            .HaveEmittedEvent(Now, "La partie de chasse est terminée, vainqueur : Dédé - 2 galinettes, Bernard - 2 galinettes")
            .And.LaPartieEstTerminée()
            .And.ChasseurATiréSurUneGalinette(Data.Dédé, 18, 2)
            .And.ChasseurATiréSurUneGalinette(Data.Bernard, 6, 2)
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

        string meilleurChasseur = UseCase.Handle(partieDeChasse.Id);
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
                .AvecUnTerrainRicheEnGalinettes(9)
                // Attention, terrain avec 3 galinette mais 4 chassées, il manque une regle métier ?
                .Avec(Dédé.AyantTué(3), Bernard.AyantTué(3), Robert.AyantTué(3))
                .AlorsQueLaPartieEst(PartieStatus.Apéro)
        );

        string meilleurChasseur = UseCase.Handle(partieDeChasse.Id);

        Repository
            .SavedPartieDeChasse()
            .Should()
            .HaveEmittedEvent(Now, "La partie de chasse est terminée, vainqueur : Dédé - 3 galinettes, Bernard - 3 galinettes, Robert - 3 galinettes")
            .And.LaPartieEstTerminée()
            .And.ChasseurATiréSurUneGalinette(Data.Dédé, 17, 3)
            .And.ChasseurATiréSurUneGalinette(Data.Bernard, 5, 3)
            .And.ChasseurATiréSurUneGalinette(Data.Robert, 9, 3);

        meilleurChasseur.Should().Be($"{Data.Dédé}, {Data.Bernard}, {Data.Robert}");
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstDéjàTerminée()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            // Attention, terrain avec 3 galinette mais 4 chassées, il manque une regle métier ?
            .Avec(Dédé, Bernard, Robert.AyantTué(2))
            .AlorsQueLaPartieEst(PartieStatus.Terminée)
        );

        var prendreLapéroQuandTerminée = () => UseCase.Handle(partieDeChasse.Id);

        prendreLapéroQuandTerminée.Should()
            .Throw<QuandCestFiniCestFini>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseNExistePas()
    {
        var prendreLapéroQuandTerminée = () => UseCase.Handle(Guid.NewGuid());

        prendreLapéroQuandTerminée.Should()
            .Throw<LaPartieDeChasseNexistePas>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}