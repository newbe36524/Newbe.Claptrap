using System;
using System.Threading.Tasks;
using Newbe.Claptrap.ScaffoldGenerator;
using Newbe.Claptrap.ScaffoldGenerator.CodeFileGenerators;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class StateFactoryCodeFileGeneratorTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public StateFactoryCodeFileGeneratorTests(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Test1()
        {
            var stateFactoryCodeFileGenerator = new StateFactoryCodeFileGenerator(typeof(TestStateDataType));
            var re = await stateFactoryCodeFileGenerator.Generate();
            _testOutputHelper.WriteCodePretty(re);

            const string target = @"using System;
using System.Threading.Tasks;
using Newbe.Claptrap;
using Newbe.Claptrap.Core;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestDataType;
namespace Claptrap._10StateDataFactory
{
    public class StateDataFactory : StateDataFactoryBase<StateData>
    {
        public StateDataFactory(IActorIdentity actorIdentity) : base(actorIdentity)
        {
        }
        public override Task<StateData> Create()
        {
            throw new NotImplementedException();
        }
    }
}";
            re.ShouldBe(target);
        }
    }
}