using System;
using System.Threading.Tasks;
using Newbe.Claptrap.ScaffoldGenerator;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.StateFactory;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class StateFactoryCodeFileGeneratorTests
        : CodeFileGeneratorTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public StateFactoryCodeFileGeneratorTests(
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
                StateDataTypeFullName = typeof(TestStateDataType).FullName,
            });
            _testOutputHelper.WriteCodePretty(re);

            AssertCodeFile(nameof(Test1), re);
        }
    }
}