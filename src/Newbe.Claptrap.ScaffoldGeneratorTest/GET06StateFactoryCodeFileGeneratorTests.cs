using System;
using System.Threading.Tasks;
using Newbe.Claptrap.ScaffoldGenerator;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE06StateFactory;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class GET06StateFactoryCodeFileGeneratorTests
        : CodeFileGeneratorTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public GET06StateFactoryCodeFileGeneratorTests(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Test1()
        {
            var stateFactoryCodeFileGenerator = new GE06CodeFileGenerator();
            var re = stateFactoryCodeFileGenerator.GenerateCode(new GE06CodeFile
            {
                StateDataTypeFullName = typeof(TestStateDataType).FullName,
            });
            _testOutputHelper.WriteCodePretty(re);

            AssertCodeFile(nameof(Test1), re);
        }
    }
}