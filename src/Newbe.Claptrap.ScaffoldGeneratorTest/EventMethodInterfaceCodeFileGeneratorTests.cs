using System;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.ScaffoldGenerator.CodeFileGenerators;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class EventMethodInterfaceCodeFileGeneratorTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EventMethodInterfaceCodeFileGeneratorTests(
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
            var methodInfo = typeof(EventMethodInterfaceCodeFileGeneratorTests)
                .GetMethod(nameof(TestTaskMethod));
            methodInfo.Should().NotBeNull();
            var generator = new EventMethodInterfaceCodeFileGenerator(typeof(TestStateDataType),
                new ClaptrapEventMethodCodeInfo(new ClaptrapEventMethodMetadata
                {
                    MethodInfo = methodInfo,
                    ClaptrapEventMetadata = new ClaptrapEventMetadata
                    {
                        EventType = "test",
                        EventDataType = typeof(TestEventDataType)
                    }
                }));
            var re = await generator.Generate();
            _testOutputHelper.WriteCodePretty(re);

            const string target = @"using Newbe.Claptrap;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.ScaffoldGeneratorTest.EventDataType;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestDataType;
namespace Claptrap._20EventMethods
{
    public interface ITestTaskMethod
    {
        Task<EventMethodResult<EventData>> Invoke(StateData stateData);
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
            var methodInfo = typeof(EventMethodInterfaceCodeFileGeneratorTests)
                .GetMethod(nameof(IntReturnMethod));
            methodInfo.Should().NotBeNull();
            var generator = new EventMethodInterfaceCodeFileGenerator(typeof(TestStateDataType),
                new ClaptrapEventMethodCodeInfo(new ClaptrapEventMethodMetadata
                {
                    MethodInfo = methodInfo,
                    ClaptrapEventMetadata = new ClaptrapEventMetadata
                    {
                        EventType = "test",
                        EventDataType = typeof(TestEventDataType)
                    }
                }));
            var re = await generator.Generate();
            _testOutputHelper.WriteCodePretty(re);

            const string target = @"using Newbe.Claptrap;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.ScaffoldGeneratorTest.EventDataType;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestDataType;
namespace Claptrap._20EventMethods
{
    public interface IIntReturnMethod
    {
       Task<EventMethodResult<EventData, System.Int32>> Invoke(StateData stateData);
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
            var methodInfo = typeof(EventMethodInterfaceCodeFileGeneratorTests)
                .GetMethod(nameof(ArgumentMethod));
            methodInfo.Should().NotBeNull();
            var generator = new EventMethodInterfaceCodeFileGenerator(typeof(TestStateDataType),
                new ClaptrapEventMethodCodeInfo(new ClaptrapEventMethodMetadata
                {
                    MethodInfo = methodInfo,
                    ClaptrapEventMetadata = new ClaptrapEventMetadata
                    {
                        EventType = "test",
                        EventDataType = typeof(TestEventDataType)
                    }
                }));
            var re = await generator.Generate();
            _testOutputHelper.WriteCodePretty(re);

            const string target = @"using Newbe.Claptrap;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.ScaffoldGeneratorTest.EventDataType;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestDataType;
namespace Claptrap._20EventMethods
{
    public interface IArgumentMethod
{
         Task<EventMethodResult<EventData>> Invoke(StateData stateData, System.String a, System.Int32 b, Newbe.Claptrap.ScaffoldGeneratorTest.EventDataType dataType);
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
            var methodInfo = typeof(EventMethodInterfaceCodeFileGeneratorTests)
                .GetMethod(nameof(IntReturnArgumentMethod));
            methodInfo.Should().NotBeNull();
            var generator = new EventMethodInterfaceCodeFileGenerator(typeof(TestStateDataType),
                new ClaptrapEventMethodCodeInfo(new ClaptrapEventMethodMetadata
                {
                    MethodInfo = methodInfo,
                    ClaptrapEventMetadata = new ClaptrapEventMetadata
                    {
                        EventType = "test",
                        EventDataType = typeof(TestEventDataType)
                    }
                }));
            var re = await generator.Generate();
            _testOutputHelper.WriteCodePretty(re);

            const string target = @"using Newbe.Claptrap;
using System.Threading.Tasks;
using EventData = Newbe.Claptrap.ScaffoldGeneratorTest.EventDataType;
using StateData = Newbe.Claptrap.ScaffoldGeneratorTest.TestDataType;
namespace Claptrap._20EventMethods
{
    public interface IIntReturnArgumentMethod
    {
Task<EventMethodResult<EventData, System.Int32>> Invoke(StateData stateData, System.String a, System.Int32 b, Newbe.Claptrap.ScaffoldGeneratorTest.EventDataType dataType);
    }
}
";
            re.ShouldBe(target);
        }
    }
}