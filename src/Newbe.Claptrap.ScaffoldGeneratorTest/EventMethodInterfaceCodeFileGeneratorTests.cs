using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.ScaffoldGenerator.CodeFileGenerators;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class EventMethodInterfaceCodeFileGeneratorTests
        : CodeFileGeneratorTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EventMethodInterfaceCodeFileGeneratorTests(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public interface ITestInterface
        {
            Task TestTaskMethod();
            Task<int> IntReturnMethod();
            Task ArgumentMethod(string a, int b, TestEventDataType dataType);
            Task<int> IntReturnArgumentMethod(string a, int b, TestEventDataType dataType);
        }

        private static MethodDeclarationSyntax GetMethodDeclarationSyntax(string methodName)
        {
            var compilationUnitSyntax = CSharpSyntaxTree.ParseText(@"
public interface ITestInterface
{
    Task TestTaskMethod();
    Task<int> IntReturnMethod();
    Task ArgumentMethod(string a, int b, TestEventDataType dataType);
    Task<int> IntReturnArgumentMethod(string a, int b, TestEventDataType dataType);
}").GetCompilationUnitRoot();
            var re = compilationUnitSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Single(x => x.Identifier.ToString() == methodName);
            return re;
        }

        [Fact]
        public async Task TestTaskMethodTest()
        {
            const string methodName = nameof(ITestInterface.TestTaskMethod);
            var methodInfo = typeof(ITestInterface)
                .GetMethod(methodName);
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
                }, GetMethodDeclarationSyntax(methodName)));
            var re = await generator.Generate();
            _testOutputHelper.WriteCodePretty(re);

            AssertCodeFile(nameof(TestTaskMethodTest), re);
        }

        [Fact]
        public async Task IntReturnMethodTest()
        {
            const string methodName = nameof(ITestInterface.IntReturnMethod);
            var methodInfo = typeof(ITestInterface)
                .GetMethod(methodName);
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
                }, GetMethodDeclarationSyntax(methodName)));
            var re = await generator.Generate();
            _testOutputHelper.WriteCodePretty(re);
            AssertCodeFile(nameof(IntReturnMethodTest), re);
        }

        [Fact]
        public async Task ArgumentMethodTest()
        {
            const string methodName = nameof(ITestInterface.ArgumentMethod);
            var methodInfo = typeof(ITestInterface)
                .GetMethod(methodName);
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
                }, GetMethodDeclarationSyntax(methodName)));
            var re = await generator.Generate();
            _testOutputHelper.WriteCodePretty(re);
            AssertCodeFile(nameof(ArgumentMethodTest), re);
        }

        [Fact]
        public async Task IntReturnArgumentMethodTest()
        {
            const string methodName = nameof(ITestInterface.IntReturnArgumentMethod);
            var methodInfo = typeof(ITestInterface)
                .GetMethod(methodName);
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
                }, GetMethodDeclarationSyntax(methodName)));
            var re = await generator.Generate();
            _testOutputHelper.WriteCodePretty(re);

            AssertCodeFile(nameof(IntReturnArgumentMethodTest), re);
        }
    }
}