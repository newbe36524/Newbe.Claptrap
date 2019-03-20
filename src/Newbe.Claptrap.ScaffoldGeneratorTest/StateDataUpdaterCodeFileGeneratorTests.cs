using System.Threading.Tasks;
using Newbe.Claptrap.ScaffoldGenerator.CodeFileGenerators;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class StateDataUpdaterCodeFileGeneratorTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public StateDataUpdaterCodeFileGeneratorTests(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Test1()
        {
            var stateFactoryCodeFileGenerator = new StateDataUpdaterCodeFileGenerator(typeof(TestStateDataType),
                typeof(TestEventDataType));
            var re = await stateFactoryCodeFileGenerator.Generate();
            _testOutputHelper.WriteCodePretty(re);

            const string target = @"using System;
using Newbe.Claptrap;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestDataType;
using EventData = Newbe.Claptrap.ScaffoldGeneratorTest.EventDataType;
namespace Claptrap._11StateDataUpdaters
{
    public class TestDataTypeUpdater : StateDataUpdaterBase<StateData, EventData>
    {
        public override void UpdateState(StateData stateData, EventData eventData)
        {
            throw new NotImplementedException();
        }
    }
}";
            re.ShouldBe(target);
        }
    }
}


