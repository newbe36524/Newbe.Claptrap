using System.Threading.Tasks;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE05StateDataUpdater;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class GET05StateDataUpdaterCodeFileGeneratorTests
        : CodeFileGeneratorTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public GET05StateDataUpdaterCodeFileGeneratorTests(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test1()
        {
            var stateFactoryCodeFileGenerator = new GE05CodeFileGenerator();
            var re = stateFactoryCodeFileGenerator.GenerateCode(new GE05CodeFile
            {
                EventDataTypeFullName = typeof(TestEventDataType).FullName,
                StateDataTypeFullName = typeof(TestStateDataType).FullName,
                ClassName = $"{typeof(TestStateDataType).Name}Updater",
            });
            _testOutputHelper.WriteCodePretty(re);

            AssertCodeFile(nameof(Test1), re);
        }
    }
}