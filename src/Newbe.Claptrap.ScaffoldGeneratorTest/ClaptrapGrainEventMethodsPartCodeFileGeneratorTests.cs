using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.ScaffoldGenerator.CodeFileGenerators;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class ClaptrapGrainEventMethodsPartCodeFileGeneratorTests
        : CodeFileGeneratorTestBase
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
            var methodInfo = GetType()
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
                    InterfaceType = typeof(ITestClaptrap),
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

            AssertCodeFile(nameof(Test1), re);
        }
    }
}