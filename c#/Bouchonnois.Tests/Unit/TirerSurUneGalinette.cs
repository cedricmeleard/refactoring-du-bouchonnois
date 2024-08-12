using Bouchonnois.Domain;
using Bouchonnois.UseCases;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.Tests.Unit;

public class TirerSurUneGalinette : PartieDeChasseServiceTest
{
    private readonly UseCases.TirerSurUneGalinette _useCase;
    public TirerSurUneGalinette()
    {
        _useCase = new UseCases.TirerSurUneGalinette(Repository, TimeProvider);
    }

    [Fact]
    public void AvecUnChasseurAyantDesBallesEtAssezDeGalinettesSurLeTerrain()
    {
        // Prepare
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            .Avec(Dédé, Bernard, Robert)
        );

        // Act
        _useCase.Handle(partieDeChasse.Id, Data.Bernard);

        // Assert
        Repository
            .SavedPartieDeChasse()
            .Should()
            .HaveEmittedEvent(Now, "Bernard tire sur une galinette")
            .And.ChasseurATiréSurUneGalinette(Data.Bernard, 7, 1)
            .And.GalinettesSurLeTerrain(2);
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = Guid.NewGuid();
        var tirerQuandPartieExistePas = () => _useCase.Handle(id, Data.Bernard);

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

        var tirerSansBalle = () => _useCase.Handle(partieDeChasse.Id, Data.Bernard);

        tirerSansBalle.Should()
            .Throw<TasPlusDeBallesMonVieuxChasseALaMain>();

        AssertLastEvent(partieDeChasse, "Bernard veut tirer sur une galinette -> T'as plus de balles mon vieux, chasse à la main");
    }

    [Fact]
    public void EchoueCarPasDeGalinetteSurLeTerrain()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(0)
            .Avec(Dédé, Bernard, Robert)
        );

        var tirerAlorsQuePasDeGalinettes = () => _useCase.Handle(partieDeChasse.Id, Data.Bernard);

        tirerAlorsQuePasDeGalinettes.Should()
            .Throw<TasTropPicoléMonVieuxTasRienTouché>();
        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueCarLeChasseurNestPasDansLaPartie()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            .Avec(Dédé, Bernard, Robert)
        );

        var chasseurInconnuVeutTirer = () => _useCase.Handle(partieDeChasse.Id, "Michel");

        chasseurInconnuVeutTirer.Should()
            .Throw<ChasseurInconnu>()
            .WithMessage("Chasseur inconnu Michel");
        Repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontEnApero()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            .Avec(Dédé, Bernard, Robert)
            .AlorsQueLaPartieEst(PartieStatus.Apéro)
        );

        var tirerEnPleinApéro = () => _useCase.Handle(partieDeChasse.Id, Data.ChasseurInconnu);

        tirerEnPleinApéro.Should()
            .Throw<OnTirePasPendantLapéroCestSacré>();

        AssertLastEvent(partieDeChasse, "Chasseur inconnu veut tirer -> On tire pas pendant l'apéro, c'est sacré !!!");
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            .Avec(Dédé, Bernard, Robert)
            .AlorsQueLaPartieEst(PartieStatus.Terminée)
        );

        var tirerQuandTerminée = () => _useCase.Handle(partieDeChasse.Id, Data.ChasseurInconnu);

        tirerQuandTerminée.Should()
            .Throw<OnTirePasQuandLaPartieEstTerminée>();

        AssertLastEvent(partieDeChasse, "Chasseur inconnu veut tirer -> On tire pas quand la partie est terminée");
    }
}