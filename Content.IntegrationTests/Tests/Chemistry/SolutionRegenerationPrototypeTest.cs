using System.Linq;
using Content.IntegrationTests.Fixtures;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.IntegrationTests.Tests.Chemistry;

[TestFixture, TestOf(typeof(SolutionRegenerationSystem))]
public sealed class SolutionRegenerationPrototypeTest : GameTest
{
    /// <summary>
    /// <see cref="SolutionRegenerationSystem"/> only ticks entities that have both
    /// <see cref="SolutionRegenerationComponent"/> and <see cref="SolutionComponent"/>.
    /// A prototype that puts SolutionRegeneration on a SolutionManager holder instead of
    /// on the solution entity itself silently never regenerates.
    /// </summary>
    [Test]
    public async Task AssertRegenerationOnSolutionEntities()
    {
        var pair = Pair;
        var server = pair.Server;

        var protoMan = server.ResolveDependency<IPrototypeManager>();
        var entMan = server.ResolveDependency<IEntityManager>();

        await server.WaitAssertion(() =>
        {
            Assert.Multiple(delegate
            {
                foreach (var prototype in protoMan.EnumeratePrototypes<EntityPrototype>()
                             .Where(p => !p.Abstract)
                             .Where(p => !pair.IsTestPrototype(p)))
                {
                    if (!prototype.TryGetComponent<SolutionRegenerationComponent>(out _, entMan.ComponentFactory))
                        continue;

                    if (!prototype.TryGetComponent<SolutionComponent>(out _, entMan.ComponentFactory))
                    {
                        Assert.Fail(
                            $"Entity prototype '{prototype.ID}' has a SolutionRegenerationComponent but no SolutionComponent, so it will never regenerate. Move SolutionRegeneration onto the solution prototype entity referenced by its SolutionManager.");
                    }
                }
            });
        });
    }
}
