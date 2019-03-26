using System.Linq;
using System.Threading.Tasks;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.EventMethodImpl;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class EventMethodImplCodeFileGeneratorTests
        : CodeFileGeneratorTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EventMethodImplCodeFileGeneratorTests(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void TestTaskMethodTest()
        {
            var generator = new CodeFileGenerator();
            var emptyStrings = Enumerable.Empty<string>().ToArray();
            var re = generator.Generate(new CodeFile
            {
                ClassName = "TestTaskMethod",
                InterfaceName = "ITestTaskMethod",
                ArgumentTypeAndNames = emptyStrings,
                EventDataTypeFullName = typeof(TestEventDataType).FullName,
                StateDataTypeFullName = typeof(TestStateDataType).FullName,
                UnwrapTaskReturnTypeName = string.Empty
            });
            _testOutputHelper.WriteCodePretty(re);
            AssertCodeFile(nameof(TestTaskMethodTest), re);
        }

        [Fact]
        public void IntReturnMethodTest()
        {
            var generator = new CodeFileGenerator();
            var emptyStrings = Enumerable.Empty<string>().ToArray();
            var re = generator.Generate(new CodeFile
            {
                ClassName = "IntReturnMethod",
                InterfaceName = "IIntReturnMethod",
                ArgumentTypeAndNames = emptyStrings,
                EventDataTypeFullName = typeof(TestEventDataType).FullName,
                StateDataTypeFullName = typeof(TestStateDataType).FullName,
                UnwrapTaskReturnTypeName = "int"
            });
            _testOutputHelper.WriteCodePretty(re);
            AssertCodeFile(nameof(IntReturnMethodTest), re);
        }

        [Fact]
        public void ArgumentMethodTest()
        {
            var generator = new CodeFileGenerator();
            var re = generator.Generate(new CodeFile
            {
                ClassName = "ArgumentMethod",
                InterfaceName = "IArgumentMethod",
                ArgumentTypeAndNames = new[] {"string a", "int b", "TestEventDataType dataType"},
                EventDataTypeFullName = typeof(TestEventDataType).FullName,
                StateDataTypeFullName = typeof(TestStateDataType).FullName,
                UnwrapTaskReturnTypeName = string.Empty
            });
            _testOutputHelper.WriteCodePretty(re);
            AssertCodeFile(nameof(ArgumentMethodTest), re);
        }

        [Fact]
        public void IntReturnArgumentMethodTest()
        {
            var generator = new CodeFileGenerator();
            var re = generator.Generate(new CodeFile
            {
                ClassName = "IntReturnArgumentMethod",
                InterfaceName = "IIntReturnArgumentMethod",
                ArgumentTypeAndNames = new[] {"string a", "int b", "TestEventDataType dataType"},
                EventDataTypeFullName = typeof(TestEventDataType).FullName,
                StateDataTypeFullName = typeof(TestStateDataType).FullName,
                UnwrapTaskReturnTypeName = "int"
            });
            _testOutputHelper.WriteCodePretty(re);
            AssertCodeFile(nameof(IntReturnArgumentMethodTest), re);
        }
    }
}