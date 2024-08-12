using Bouchonnois.Domain;
using Bouchonnois.Service.Exceptions;
using Bouchonnois.Tests.Builders;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.Service.PartieDeChasseService.UseCases;

public class ReprendreLaPartieDeChasse
{
    [Fact]
    public void QuandLapéroEstEnCours()
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
        service.ReprendreLaPartie(id);

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
        savedPartieDeChasse.Chasseurs[1].BallesRestantes.Should().Be(8);
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
        var reprendrePartieQuandPartieExistePas = () => service.ReprendreLaPartie(id);

        reprendrePartieQuandPartieExistePas.Should()
            .Throw<LaPartieDeChasseNexistePas>();
        repository.SavedPartieDeChasse().Should().BeNull();
    }

    [Fact]
    public void EchoueSiLaChasseEstEnCours()
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
        var reprendreLaPartieQuandChasseEnCours = () => service.ReprendreLaPartie(id);

        reprendreLaPartieQuandChasseEnCours.Should()
            .Throw<LaChasseEstDéjàEnCours>();

        repository.SavedPartieDeChasse().Should().BeNull();
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
        var prendreLapéroQuandTerminée = () => service.ReprendreLaPartie(id);

        prendreLapéroQuandTerminée.Should()
            .Throw<QuandCestFiniCestFini>();

        repository.SavedPartieDeChasse().Should().BeNull();
    }
}