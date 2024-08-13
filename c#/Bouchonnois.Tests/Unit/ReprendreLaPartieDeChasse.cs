using Bouchonnois.Domain;
using Bouchonnois.Domain.Exceptions;
using Bouchonnois.UseCases;
using Bouchonnois.UseCases.Exceptions;

namespace Bouchonnois.Tests.Unit;

public class ReprendreLaPartieDeChasse : UseCaseTest<ReprendreLaPartie>
{
    public ReprendreLaPartieDeChasse() : base((r, t) => new ReprendreLaPartie(r, t))
    {
    }

    [Fact]
    public void QuandLapéroEstEnCours()
    {
        var partieDeChasse = AvecUnePartieDeChasseExistante(NouvellePartieDeChasse
            .AvecUnTerrainRicheEnGalinettes(3)
            .Avec(Dédé, Bernard, Robert)
            .AlorsQueLaPartieEst(PartieStatus.Apéro)
        );

        UseCase.Handle(partieDeChasse.Id);

        Repository
            .SavedPartieDeChasse()
            .Should()
            .HaveEmittedEvent(Now, "Reprise de la chasse")
            .And.GalinettesSurLeTerrain(3)
            .And.LaPartieEstEnCours()
            .And.ChasseurATiréSurUneGalinette(Data.Dédé, 20, 0)
            .And.ChasseurATiréSurUneGalinette(Data.Bernard, 8, 0)
            .And.ChasseurATiréSurUneGalinette(Data.Robert, 12, 0);
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = Guid.NewGuid();
        var reprendrePartieQuandPartieExistePas = () => UseCase.Handle(id);

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

        var reprendreLaPartieQuandChasseEnCours = () => UseCase.Handle(partieDeChasse.Id);

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

        var prendreLapéroQuandTerminée = () => UseCase.Handle(partieDeChasse.Id);

        prendreLapéroQuandTerminée.Should()
            .Throw<QuandCestFiniCestFini>();

        Repository.SavedPartieDeChasse().Should().BeNull();
    }
}