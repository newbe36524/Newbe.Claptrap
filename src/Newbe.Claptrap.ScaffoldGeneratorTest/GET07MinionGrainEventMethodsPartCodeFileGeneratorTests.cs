using System.Linq;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE07MinionGrainEventMethodsPart;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class GET07MinionGrainEventMethodsPartCodeFileGeneratorTests
        : CodeFileGeneratorTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public GET07MinionGrainEventMethodsPartCodeFileGeneratorTests(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CodeFileTest()
        {
            var generator = new GE07CodeFileGenerator();
            var re = generator.GenerateCode(new GE07CodeFile
            {
                ClassName = "TestClaptrap",
                InterfaceName = "ITestClaptrap",
                ClaptrapCatalog = "TestClaptrap",
                StateDataTypeFullName = typeof(TestStateDataType).FullName,
                Namespaces = Enumerable.Empty<string>().ToArray(),
                MinionCatalog = "Database",
                EventMethods = new[]
                {
                    "TestTaskEvent",
                    "TestTaskEvent2"
                },
            });
            _testOutputHelper.WriteCodePretty(re);

            AssertCodeFile(nameof(CodeFileTest), re);
        }
    }
}