using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.Orleans;
using Newbe.Claptrap.ScaffoldGenerator.CodeFileGenerators;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class ClaptrapGrainEventMethodsPartCodeFileGeneratorTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ClaptrapGrainEventMethodsPartCodeFileGeneratorTests(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public Task TestTaskMethod()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task Test1()
        {
            var methodInfo = this.GetType()
                .GetMethod(nameof(TestTaskMethod));
            var claptrapEventMetadata = new ClaptrapEventMetadata
            {
                EventType = "TestEventType",
                EventDataType = typeof(TestEventDataType),
            };
            var stateFactoryCodeFileGenerator = new ClaptrapGrainEventMethodsPartCodeFileGenerator(
                new ClaptrapMetadata
                {
                    ClaptrapKind = new ClaptrapKind(ActorType.Claptrap, "TestCatalog"),
                    InterfaceType = typeof(ITestClaptrapGrain),
                    MinionMetadata = Enumerable.Empty<MinionMetadata>(),
                    StateDataType = typeof(TestStateDataType),
                    EventMethodMetadata = new[]
                    {
                        new ClaptrapEventMethodMetadata
                        {
                            MethodInfo = methodInfo,
                            ClaptrapEventMetadata = claptrapEventMetadata
                        },
                    },
                    ClaptrapEventMetadata = new[]
                    {
                        claptrapEventMetadata
                    },
                    NoneEventMethodInfos = Enumerable.Empty<MethodInfo>()
                });
            var re = await stateFactoryCodeFileGenerator.Generate();
            _testOutputHelper.WriteCodePretty(re);

            var target = await File.ReadAllTextAsync("CodeFiles/ClaptrapGrainEventMethodsPartCodeFileGeneratorTests/Test1.cs");
            re.ShouldBe(target);
        }
    }

    public interface ITestClaptrapGrain : IClaptrapGrain
    {
    }
}