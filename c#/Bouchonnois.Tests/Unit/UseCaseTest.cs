using Bouchonnois.Domain;
using Bouchonnois.Tests.Builders;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.Unit;

public abstract class UseCaseTestBase
{
    protected static readonly DateTime Now = new(2024, 6, 6, 14, 50, 45);
    protected static readonly Func<DateTime> TimeProvider = () => Now;
    protected static List<(string, int)> PasDeChasseurs => [];
}

public class UseCaseTest<TUseCase> : UseCaseTestBase
{
    protected readonly PartieDeChasseRepositoryForTests Repository;
    protected readonly TUseCase UseCase;

    protected UseCaseTest(Func<IPartieDeChasseRepository, Func<DateTime>, TUseCase> useCaseFactory)
    {
        Repository = new PartieDeChasseRepositoryForTests();
        UseCase = useCaseFactory(Repository, TimeProvider);
    }

    protected static void AssertLastEvent(PartieDeChasse partieDeChasse, string expectedMessage)
    {
        partieDeChasse
            .Events.Should()
            .EndWith(new Event(Now, expectedMessage));
    }

    protected PartieDeChasse AvecUnePartieDeChasseExistante(PartieDeChasseBuilder partieDeChasseBuilder)
    {
        var partieDeChasse = partieDeChasseBuilder.Build(TimeProvider, Repository);
        Repository.Add(partieDeChasse);

        return partieDeChasse;
    }

    protected bool MustFailWith<TException>(Action action, Func<PartieDeChasse?, bool>? assert = null)
        where TException : Exception
    {
        try
        {
            action();
            return false;
        }
        catch (TException)
        {
            return assert?.Invoke(Repository.SavedPartieDeChasse()) ?? true;
        }
    }
}