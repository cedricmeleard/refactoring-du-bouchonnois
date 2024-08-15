using Bouchonnois.Domain;
using Bouchonnois.Domain.Exceptions;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.Tests.Unit;

public class Tirer : UseCaseTest<UseCases.Tirer>
{
    public Tirer() : base((r, t) => new UseCases.Tirer(r, t))
    {
    }


    [Fact]
    public void AvecUnChasseurAyantDesBalles()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            .Avec(Dédé, Bernard, Robert)
        );

        UseCase.Handle(partieDeChasse.Id, Data.Bernard);

        Repository
            .SavedPartieDeChasse()
            .Should()
            .HaveEmittedEvent(Now, "Bernard tire")
            .And.LaPartieEstEnCours()
            .And.ChasseurATiréSurUneGalinette(Data.Dédé, 20, 0)
            .And.ChasseurATiréSurUneGalinette(Data.Bernard, 7, 0)
            .And.ChasseurATiréSurUneGalinette(Data.Robert, 12, 0)
            .And.GalinettesSurLeTerrain(3);
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = Guid.NewGuid();
        var tirerQuandPartieExistePas = () => UseCase.Handle(id, Data.Bernard);

        tirerQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueAvecUnChasseurNayantPlusDeBalles()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            .Avec(Dédé, Bernard.SansBalle(), Robert)
        );

        var tirerSansBalle = () => UseCase.Handle(partieDeChasse.Id, Data.Bernard);

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

        var chasseurInconnuVeutTirer = () => UseCase.Handle(partieDeChasse.Id, "Chasseur inconnu");

        chasseurInconnuVeutTirer.Should()
            .Throw<ChasseurInconnu>();
        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Theory]
    [InlineData(Data.Bernard)]
    [InlineData(Data.ChasseurInconnu)]
    [InlineData("Michel")]
    public void EchoueSiLesChasseursSontEnApero(string name)
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            .Avec(Dédé, Bernard, Robert)
            .AlorsQueLaPartieEst(PartieStatus.Apéro)
        );

        var tirerEnPleinApéro = () => UseCase.Handle(partieDeChasse.Id, name);

        tirerEnPleinApéro.Should()
            .Throw<OnTirePasPendantLapéroCestSacré>();

        AssertLastEvent(partieDeChasse, $"{name} veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!");
    }

    [Theory]
    [InlineData(Data.Bernard)]
    [InlineData(Data.ChasseurInconnu)]
    [InlineData("Michel")]
    public void EchoueSiLaPartieDeChasseEstTerminée(string name)
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            .Avec(Dédé, Bernard, Robert)
            .AlorsQueLaPartieEst(PartieStatus.Terminée)
        );

        var tirerQuandTerminée = () => UseCase.Handle(partieDeChasse.Id, name);

        tirerQuandTerminée.Should()
            .Throw<OnTirePasQuandLaPartieEstTerminée>();

        AssertLastEvent(partieDeChasse, $"{name} veut tirer -> On tire pas quand la partie est terminée");
    }
}