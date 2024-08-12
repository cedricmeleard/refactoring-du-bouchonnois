using ArchUnitNET.Fluent;
using ArchUnitNET.Fluent.Syntax.Elements.Members.MethodMembers;
using ArchUnitNET.Fluent.Syntax.Elements.Types.Interfaces;

namespace Bouchonnois.Tests.Architecture;

public class Guidelines
{
    private static GivenMethodMembersThat Methods() => ArchRuleDefinition.MethodMembers().That().AreNoConstructors().And();
    private static GivenInterfaces Interfaces() => ArchRuleDefinition.Interfaces();

    [Fact]
    public void NoGetMethodShouldReturnVoid() =>
        Methods()
            .HaveName("Get[A-Z].*", true).Should()
            .NotHaveReturnType(typeof(void))
            .Check();

    [Fact]
    public void IserAndHaserShouldReturnBooleans() =>
        Methods()
            .HaveName("Is[A-Z].*", true).Or()
            .HaveName("Has[A-Z].*", true).Should()
            .HaveReturnType(typeof(bool))
            .Check();

    [Fact]
    public void SettersShouldNotReturnSomething() =>
        Methods()
            .HaveName("Set[A-Z].*", true).Should()
            .HaveReturnType(typeof(void))
            .Check();

    [Fact]
    public void InterfacesShouldStartWithI() =>
        Interfaces().Should()
            .HaveName("^I[A-Z].*", true)
            .Because("C# convention...")
            .Check();
}