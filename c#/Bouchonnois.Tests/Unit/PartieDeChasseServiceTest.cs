using Bouchonnois.Domain;
using Bouchonnois.Service;
using Bouchonnois.Tests.Builders;
using Bouchonnois.Tests.Doubles;

namespace Bouchonnois.Tests.Unit;

public abstract class PartieDeChasseServiceTest
{
    protected readonly static DateTime Now = new(2024, 6, 6, 14, 50, 45);
    private readonly static Func<DateTime> TimeProvider = () => Now;

    protected readonly PartieDeChasseService PartieDeChasseService;
    protected ChasseurBuilder Dédé => Dédé();
    protected ChasseurBuilder Bernard => Bernard();
    protected ChasseurBuilder Robert => Robert();
    protected PartieDeChasseBuilder NouvellePartieDeChasse => new();

    protected readonly PartieDeChasseRepositoryForTests Repository;
    protected PartieDeChasseServiceTest()
    {
        Repository = new PartieDeChasseRepositoryForTests();
        PartieDeChasseService = new PartieDeChasseService(Repository, TimeProvider);
    }
    protected static void AssertLastEvent(PartieDeChasse partieDeChasse, string expectedMessage)
    {
        partieDeChasse
            .Events.Should()
            .HaveCount(1)
            .And
            .EndWith(new Event(Now, expectedMessage));
    }

    protected PartieDeChasse AvecUnePartieDeChasseExistante(PartieDeChasseBuilder partieDeChasseBuilder)
    {
        var partieDeChasse = partieDeChasseBuilder.Build();
        Repository.Add(partieDeChasse);

        return partieDeChasse;
    }
}