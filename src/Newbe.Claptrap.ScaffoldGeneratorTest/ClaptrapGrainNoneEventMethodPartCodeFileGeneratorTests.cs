using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.ClaptrapGrainNoneEventMethodPart;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class ClaptrapGrainNoneEventMethodPartCodeFileGeneratorTests
        : CodeFileGeneratorTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ClaptrapGrainNoneEventMethodPartCodeFileGeneratorTests(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void NoneEventMethodWithoutReturnValue()
        {
            var claptrapGrainNoneEventMethodPartCodeFileGenerator = new CodeFileGenerator();
            var emptyStrings = Enumerable.Empty<string>().ToArray();
            var re = claptrapGrainNoneEventMethodPartCodeFileGenerator.Generate(new CodeFile
            {
                ClassName = "TestClaptrap",
                NoneEventMethods = new[]
                {
                    new NoneEventMethod
                    {
                        MethodName = "NoneEventMethodWithoutReturnValue",
                        ReturnTypeName = "Task",
                        ArgumentTypeAndNames = emptyStrings
                    },
                    new NoneEventMethod
                    {
                        MethodName = "NoneEventMethodWithReturnValue",
                        ReturnTypeName = "Task<DateTime>",
                        ArgumentTypeAndNames = emptyStrings,
                    },
                    new NoneEventMethod
                    {
                        MethodName = "AddBalance",
                        ReturnTypeName = "Task",
                        ArgumentTypeAndNames = new[] {"decimal value"}
                    },
                    new NoneEventMethod
                    {
                        MethodName = "SomeMethod",
                        ReturnTypeName = "Task<(decimal value, int n)>",
                        ArgumentTypeAndNames = new[] {"TestEventDataType testEventDataType"}
                    },
                }
            });
            _testOutputHelper.WriteCodePretty(re);
            AssertCodeFile(nameof(NoneEventMethodWithoutReturnValue), re);
        }
    }
}