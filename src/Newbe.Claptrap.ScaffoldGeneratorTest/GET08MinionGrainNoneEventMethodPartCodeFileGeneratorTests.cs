using System.Linq;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE08MinionGrainNoneEventMethodPart;
using Xunit;
using Xunit.Abstractions;
using NoneEventMethod = Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE08MinionGrainNoneEventMethodPart.NoneEventMethod;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class GET08MinionGrainNoneEventMethodPartCodeFileGeneratorTests
        : CodeFileGeneratorTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public GET08MinionGrainNoneEventMethodPartCodeFileGeneratorTests(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        
        [Fact]
        public void NoneEventMethodWithoutReturnValue()
        {
            var claptrapGrainNoneEventMethodPartCodeFileGenerator = new GE08CodeFileGenerator();
            var emptyStrings = Enumerable.Empty<string>().ToArray();
            var re = claptrapGrainNoneEventMethodPartCodeFileGenerator.GenerateCode(new GE08CodeFile
            {
                ClassName = "TestClaptrap",
                Namespaces = Enumerable.Empty<string>().ToArray(),
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
                },
            });
            _testOutputHelper.WriteCodePretty(re);
            AssertCodeFile(nameof(NoneEventMethodWithoutReturnValue), re);
        }
    }
}