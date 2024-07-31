using Bouchonnois.Domain;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Builders;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.Service.PartieDeChasseService.UseCases;

public class Tirer
{
    [Fact]
    public void AvecUnChasseurAyantDesBalles()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .WithId(id)
            .WithTerrain(TestConstants.TerrainName, 3)
            .WithChasseur("Dédé", 20)
            .WithChasseur("Bernard", 8)
            .WithChasseur("Robert", 12)
            .Build();

        repository.Add(partieDeChasse);

        var service = new Bouchonnois.Service.PartieDeChasseService(repository, () => DateTime.Now);

        service.Tirer(id, "Bernard");

        var savedPartieDeChasse = repository.SavedPartieDeChasse();
        savedPartieDeChasse.Id.Should().Be(id);
        savedPartieDeChasse.Status.Should().Be(PartieStatus.EnCours);
        savedPartieDeChasse.Terrain.Nom.Should().Be(TestConstants.TerrainName);
        savedPartieDeChasse.Terrain.NbGalinettes.Should().Be(3);
        savedPartieDeChasse.Chasseurs.Should().HaveCount(3);
        savedPartieDeChasse.Chasseurs[0].Nom.Should().Be("Dédé");
        savedPartieDeChasse.Chasseurs[0].BallesRestantes.Should().Be(20);
        savedPartieDeChasse.Chasseurs[0].NbGalinettes.Should().Be(0);
        savedPartieDeChasse.Chasseurs[1].Nom.Should().Be("Bernard");
        savedPartieDeChasse.Chasseurs[1].BallesRestantes.Should().Be(7);
        savedPartieDeChasse.Chasseurs[1].NbGalinettes.Should().Be(0);
        savedPartieDeChasse.Chasseurs[2].Nom.Should().Be("Robert");
        savedPartieDeChasse.Chasseurs[2].BallesRestantes.Should().Be(12);
        savedPartieDeChasse.Chasseurs[2].NbGalinettes.Should().Be(0);
    }

    [Fact]
    public void EchoueCarPartieNexistePas()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();
        var service = new Bouchonnois.Service.PartieDeChasseService(repository, () => DateTime.Now);
        var tirerQuandPartieExistePas = () => service.Tirer(id, "Bernard");

        tirerQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueAvecUnChasseurNayantPlusDeBalles()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .WithId(id)
            .WithTerrain(TestConstants.TerrainName, 3)
            .WithChasseur("Dédé", 20)
            .WithChasseur("Bernard", 0)
            .WithChasseur("Robert", 12)
            .Build();

        repository.Add(partieDeChasse);

        var service = new Bouchonnois.Service.PartieDeChasseService(repository, () => DateTime.Now);
        var tirerSansBalle = () => service.Tirer(id, "Bernard");

        tirerSansBalle.Should()
            .Throw<TasPlusDeBallesMonVieuxChasseALaMain>();
    }

    [Fact]
    public void EchoueCarLeChasseurNestPasDansLaPartie()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .WithId(id)
            .WithTerrain(TestConstants.TerrainName, 3)
            .WithChasseur("Dédé", 20)
            .WithChasseur("Bernard", 8)
            .WithChasseur("Robert", 12)
            .Build();

        repository.Add(partieDeChasse);

        var service = new Bouchonnois.Service.PartieDeChasseService(repository, () => DateTime.Now);
        var chasseurInconnuVeutTirer = () => service.Tirer(id, "Chasseur inconnu");

        chasseurInconnuVeutTirer.Should()
            .Throw<ChasseurInconnu>();
        repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLesChasseursSontEnApero()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .WithId(id)
            .WithTerrain(TestConstants.TerrainName, 3)
            .WithChasseur("Dédé", 20)
            .WithChasseur("Bernard", 8)
            .WithChasseur("Robert", 12)
            .WithPartieStatus(PartieStatus.Apéro)
            .Build();

        repository.Add(partieDeChasse);

        var service = new Bouchonnois.Service.PartieDeChasseService(repository, () => DateTime.Now);
        var tirerEnPleinApéro = () => service.Tirer(id, "Chasseur inconnu");

        tirerEnPleinApéro.Should()
            .Throw<OnTirePasPendantLapéroCestSacré>();
    }

    [Fact]
    public void EchoueSiLaPartieDeChasseEstTerminée()
    {
        var id = Guid.NewGuid();
        var repository = new PartieDeChasseRepositoryForTests();

        var partieDeChasse = new PartieDeChasseBuilder()
            .WithId(id)
            .WithTerrain(TestConstants.TerrainName, 3)
            .WithChasseur("Dédé", 20)
            .WithChasseur("Bernard", 8)
            .WithChasseur("Robert", 12)
            .WithPartieStatus(PartieStatus.Terminée)
            .Build();

        repository.Add(partieDeChasse);

        var service = new Bouchonnois.Service.PartieDeChasseService(repository, () => DateTime.Now);
        var tirerQuandTerminée = () => service.Tirer(id, "Chasseur inconnu");

        tirerQuandTerminée.Should()
            .Throw<OnTirePasQuandLaPartieEstTerminée>();
    }
}