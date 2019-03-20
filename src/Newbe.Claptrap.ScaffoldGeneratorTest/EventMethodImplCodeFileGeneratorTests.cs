using System;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Newbe.Claptrap.ScaffoldGenerator.CodeFileGenerators;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class EventMethodImplCodeFileGeneratorTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EventMethodImplCodeFileGeneratorTests(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public Task TestTaskMethod()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task TestTaskMethodTest()
        {
            var methodInfo = this.GetType()
                .GetMethod(nameof(TestTaskMethod));
            methodInfo.Should().NotBeNull();
            var generator = new EventMethodImplCodeFileGenerator(typeof(TestStateDataType),
                typeof(TestEventDataType),
                methodInfo);
            var re = await generator.Generate();
            _testOutputHelper.WriteCodePretty(re);

            const string target = @"using Newbe.Claptrap;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.ScaffoldGeneratorTest.EventDataType;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestDataType;
namespace Claptrap._20EventMethods
{
    public class TestTaskMethod:ITestTaskMethod
    {
       public Task<EventMethodResult<EventData>> Invoke(StateData stateData)
{
throw new NotImplementedException();
}
    }
}
";
            re.ShouldBe(target);
        }

        public Task<int> IntReturnMethod()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task IntReturnMethodTest()
        {
            var methodInfo = this.GetType()
                .GetMethod(nameof(IntReturnMethod));
            methodInfo.Should().NotBeNull();
            var generator = new EventMethodImplCodeFileGenerator(typeof(TestStateDataType),
                typeof(TestEventDataType),
                methodInfo);
            var re = await generator.Generate();
            _testOutputHelper.WriteCodePretty(re);

            const string target = @"using Newbe.Claptrap;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.ScaffoldGeneratorTest.EventDataType;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestDataType;
namespace Claptrap._20EventMethods
{
    public class IntReturnMethod:IIntReturnMethod
    {
      public Task<EventMethodResult<EventData, System.Int32>> Invoke(StateData stateData)
{
throw new NotImplementedException();
}
    }
}
";
            re.ShouldBe(target);
        }

        public Task ArgumentMethod(string a, int b, TestEventDataType dataType)
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task ArgumentMethodTest()
        {
            var methodInfo = this.GetType()
                .GetMethod(nameof(ArgumentMethod));
            methodInfo.Should().NotBeNull();
            var generator = new EventMethodImplCodeFileGenerator(typeof(TestStateDataType),
                typeof(TestEventDataType),
                methodInfo);
            var re = await generator.Generate();
            _testOutputHelper.WriteCodePretty(re);

            const string target = @"using Newbe.Claptrap;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.ScaffoldGeneratorTest.EventDataType;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestDataType;
namespace Claptrap._20EventMethods
{
    public class ArgumentMethod:IArgumentMethod
{
        public  Task<EventMethodResult<EventData>> Invoke(StateData stateData, System.String a, System.Int32 b, Newbe.Claptrap.ScaffoldGeneratorTest.EventDataType dataType)
{
throw new NotImplementedException();
}
    }
}
";
            re.ShouldBe(target);
        }

        public Task<int> IntReturnArgumentMethod(string a, int b, TestEventDataType dataType)
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task IntReturnArgumentMethodTest()
        {
            var methodInfo = this.GetType()
                .GetMethod(nameof(IntReturnArgumentMethod));
            methodInfo.Should().NotBeNull();
            var generator = new EventMethodImplCodeFileGenerator(typeof(TestStateDataType),
                typeof(TestEventDataType),
                methodInfo);
            var re = await generator.Generate();
            _testOutputHelper.WriteCodePretty(re);

            const string target = @"using Newbe.Claptrap;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.ScaffoldGeneratorTest.EventDataType;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestDataType;
namespace Claptrap._20EventMethods
{
    public class IntReturnArgumentMethod:IIntReturnArgumentMethod
    {
public Task<EventMethodResult<EventData, System.Int32>> Invoke(StateData stateData, System.String a, System.Int32 b, Newbe.Claptrap.ScaffoldGeneratorTest.EventDataType dataType)
{
throw new NotImplementedException();
}
    }
}
";
            re.ShouldBe(target);
        }
    }
}