using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.ClaptrapGrainEventMethodsPart;
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

        [Fact]
        public void CodeFileTest()
        {
            var generator = new CodeFileGenerator();
            var re = generator.Generate(new CodeFile
            {
                ClassName = "TestClaptrap",
                InterfaceName = "ITestClaptrap",
                ClaptrapCatalog = "TestClaptrap",
                StateDataTypeFullName = typeof(TestStateDataType).FullName,
                EventMethods = new[]
                {
                    new EventMethod
                    {
                        MethodName = "TestTaskMethod",
                        ReturnType = "Task",
                        ArgumentNames = new[]
                        {
                            "a", "b", "c"
                        },
                        ArgumentTypeAndNames = new[]
                        {
                            "int a", "string b", "DateTime c"
                        },
                        EventType = typeof(TestEventDataType).FullName,
                        EventMethodInterfaceName = "ITestTaskMethod",
                    },
                }
            });
            _testOutputHelper.WriteCodePretty(re);

            AssertCodeFile(nameof(CodeFileTest), re);
        }
    }
}