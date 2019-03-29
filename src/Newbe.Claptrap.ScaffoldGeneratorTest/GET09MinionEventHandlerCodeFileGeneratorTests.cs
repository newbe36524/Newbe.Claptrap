using System.Linq;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE05StateDataUpdater;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE09MinionEventHandler;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class GET09MinionEventHandlerCodeFileGeneratorTests
        : CodeFileGeneratorTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public GET09MinionEventHandlerCodeFileGeneratorTests(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test1()
        {
            var stateFactoryCodeFileGenerator = new GE09CodeFileGenerator();
            var re = stateFactoryCodeFileGenerator.GenerateCode(new GE09CodeFile
            {
                Namespaces = Enumerable.Empty<string>().ToArray(),
                EventDataTypeFullName = typeof(TestEventDataType).FullName,
                StateDataTypeFullName = typeof(TestStateDataType).FullName,
                ClassName = $"{typeof(TestStateDataType).Name}EventHandler",
            });
            _testOutputHelper.WriteCodePretty(re);

            AssertCodeFile(nameof(Test1), re);
        }
    }
}