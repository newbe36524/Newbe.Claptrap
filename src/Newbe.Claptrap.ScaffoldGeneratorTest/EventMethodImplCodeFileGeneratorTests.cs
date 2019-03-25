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
        : CodeFileGeneratorTestBase
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
            var methodInfo = GetType()
                .GetMethod(nameof(TestTaskMethod));
            methodInfo.Should().NotBeNull();
            var generator = new EventMethodImplCodeFileGenerator(typeof(TestStateDataType),
                typeof(TestEventDataType),
                methodInfo);
            var re = await generator.Generate();
            _testOutputHelper.WriteCodePretty(re);
            AssertCodeFile(nameof(TestTaskMethodTest), re);
        }

        public Task<int> IntReturnMethod()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task IntReturnMethodTest()
        {
            var methodInfo = GetType()
                .GetMethod(nameof(IntReturnMethod));
            methodInfo.Should().NotBeNull();
            var generator = new EventMethodImplCodeFileGenerator(typeof(TestStateDataType),
                typeof(TestEventDataType),
                methodInfo);
            var re = await generator.Generate();
            _testOutputHelper.WriteCodePretty(re);
            AssertCodeFile(nameof(IntReturnMethodTest), re);
        }

        public Task ArgumentMethod(string a, int b, TestEventDataType dataType)
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task ArgumentMethodTest()
        {
            var methodInfo = GetType()
                .GetMethod(nameof(ArgumentMethod));
            methodInfo.Should().NotBeNull();
            var generator = new EventMethodImplCodeFileGenerator(typeof(TestStateDataType),
                typeof(TestEventDataType),
                methodInfo);
            var re = await generator.Generate();
            _testOutputHelper.WriteCodePretty(re);

            AssertCodeFile(nameof(ArgumentMethodTest), re);
        }

        public Task<int> IntReturnArgumentMethod(string a, int b, TestEventDataType dataType)
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task IntReturnArgumentMethodTest()
        {
            var methodInfo = GetType()
                .GetMethod(nameof(IntReturnArgumentMethod));
            methodInfo.Should().NotBeNull();
            var generator = new EventMethodImplCodeFileGenerator(typeof(TestStateDataType),
                typeof(TestEventDataType),
                methodInfo);
            var re = await generator.Generate();
            _testOutputHelper.WriteCodePretty(re);

            AssertCodeFile(nameof(IntReturnArgumentMethodTest), re);
        }
    }
}