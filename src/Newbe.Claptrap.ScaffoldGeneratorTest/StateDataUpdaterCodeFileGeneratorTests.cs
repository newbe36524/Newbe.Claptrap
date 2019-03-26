using System.Threading.Tasks;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.StateDataUpdater;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class StateDataUpdaterCodeFileGeneratorTests
        : CodeFileGeneratorTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public StateDataUpdaterCodeFileGeneratorTests(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test1()
        {
            var stateFactoryCodeFileGenerator = new CodeFileGenerator();
            var re = stateFactoryCodeFileGenerator.Generate(new CodeFile
            {
                EventDataTypeFullName = typeof(TestEventDataType).FullName,
                StateDataTypeFullName = typeof(TestStateDataType).FullName,
                StateDataName = typeof(TestStateDataType).Name
            });
            _testOutputHelper.WriteCodePretty(re);

            AssertCodeFile(nameof(Test1), re);
        }
    }
}