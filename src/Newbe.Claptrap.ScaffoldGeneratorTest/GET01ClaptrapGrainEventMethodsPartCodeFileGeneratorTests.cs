using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE01ClaptrapGrainEventMethodsPart;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class GET01ClaptrapGrainEventMethodsPartCodeFileGeneratorTests
        : CodeFileGeneratorTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public GET01ClaptrapGrainEventMethodsPartCodeFileGeneratorTests(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void CodeFileTest()
        {
            var generator = new GE01CodeFileGenerator();
            var emptyStrings = Enumerable.Empty<string>().ToArray();
            var re = generator.GenerateCode(new GE01CodeFile
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
                    new EventMethod
                    {
                        MethodName = "TestTaskMethod2",
                        ReturnType = "Task",
                        ArgumentNames = emptyStrings,
                        ArgumentTypeAndNames = emptyStrings,
                        EventType = typeof(TestEventDataType).FullName,
                        EventMethodInterfaceName = "ITestTaskMethod2",
                    },
                }
            });
            _testOutputHelper.WriteCodePretty(re);

            AssertCodeFile(nameof(CodeFileTest), re);
        }
    }
}