using Bouchonnois.Tests.Builders;
using Bouchonnois.Tests.Doubles;
using Bouchonnois.UseCases;
using FluentAssertions.Extensions;

namespace Bouchonnois.Tests.Acceptance;

public class ScenarioTests
{
    private readonly ConsulterStatusUseCase _consulterStatus;
    private readonly DemarrerUnePartieDeChasseUseCase _demarrerPartieDeChasse;
    private readonly PrendreLAperoUseCase _prendreLapéro;
    private readonly ReprendreLaPartieUseCase _reprendreLaPartie;
    private readonly TerminerLaPartieUseCase _terminerLaPartie;
    private readonly TirerUseCase _tirer;
    private readonly TirerSurUneGalinetteUseCase _tirerSurUneGalinette;
    private DateTime _time = new(2024, 4, 25, 9, 0, 0);

    public ScenarioTests()
    {
        var repository = new PartieDeChasseRepositoryForTests();
        var timeProvider = () => _time;

        _demarrerPartieDeChasse = new DemarrerUnePartieDeChasseUseCase(repository, timeProvider);
        _tirer = new TirerUseCase(repository, timeProvider);
        _tirerSurUneGalinette = new TirerSurUneGalinetteUseCase(repository, timeProvider);
        _prendreLapéro = new PrendreLAperoUseCase(repository, timeProvider);
        _reprendreLaPartie = new ReprendreLaPartieUseCase(repository, timeProvider);
        _terminerLaPartie = new TerminerLaPartieUseCase(repository, timeProvider);
        _consulterStatus = new ConsulterStatusUseCase(repository);
    }

    [Fact]
    public Task DéroulerUnePartie()
    {
        var command = DémarrerUnePartieDeChasse()
            .Avec((Data.Dédé, 20), (Data.Bernard, 8), (Data.Robert, 12))
            .SurUnTerrainRicheEnGalinettes(4);

        var id = _demarrerPartieDeChasse.Demarrer(
            command.Terrain,
            command.Chasseurs
        );

        After(10.Minutes(), () => _tirer.Tirer(id, Data.Dédé));
        After(30.Minutes(), () => _tirerSurUneGalinette.TirerSurUneGalinette(id, Data.Robert));
        After(20.Minutes(), () => _prendreLapéro.PrendreLapéro(id));
        After(1.Hours(), () => _reprendreLaPartie.ReprendreLaPartie(id));
        After(2.Minutes(), () => _tirer.Tirer(id, Data.Bernard));
        After(1.Minutes(), () => _tirer.Tirer(id, Data.Bernard));
        After(1.Minutes(), () => _tirerSurUneGalinette.TirerSurUneGalinette(id, Data.Dédé));
        After(26.Minutes(), () => _tirerSurUneGalinette.TirerSurUneGalinette(id, Data.Robert));
        After(10.Minutes(), () => _prendreLapéro.PrendreLapéro(id));
        After(170.Minutes(), () => _reprendreLaPartie.ReprendreLaPartie(id));
        After(11.Minutes(), () => _tirer.Tirer(id, Data.Bernard));
        After(1.Seconds(), () => _tirer.Tirer(id, Data.Bernard));
        After(1.Seconds(), () => _tirer.Tirer(id, Data.Bernard));
        After(1.Seconds(), () => _tirer.Tirer(id, Data.Bernard));
        After(1.Seconds(), () => _tirer.Tirer(id, Data.Bernard));
        After(1.Seconds(), () => _tirer.Tirer(id, Data.Bernard));
        After(1.Seconds(), () => _tirer.Tirer(id, Data.Bernard));
        After(19.Minutes(), () => _tirerSurUneGalinette.TirerSurUneGalinette(id, Data.Robert));
        After(30.Minutes(), () => _terminerLaPartie.TerminerLaPartie(id));

        return Verify(_consulterStatus.ConsulterStatus(id));
    }
    private void After(TimeSpan timeToAdd, Action act)
    {
        _time = _time.Add(timeToAdd);
        try {
            act();
        }
        catch {
            // do nothing
        }
    }
    private PartieDeChasseCommandBuilder DémarrerUnePartieDeChasse()
    {
        return new PartieDeChasseCommandBuilder();
    }
}