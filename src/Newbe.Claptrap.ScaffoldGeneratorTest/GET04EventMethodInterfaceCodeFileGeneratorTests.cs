using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE04EventMethodInterface;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class GET04EventMethodInterfaceCodeFileGeneratorTests
        : CodeFileGeneratorTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public GET04EventMethodInterfaceCodeFileGeneratorTests(
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
        public void TestTaskMethodTest()
        {
            const string methodName = nameof(ITestInterface.TestTaskMethod);
            var methodInfo = typeof(ITestInterface)
                .GetMethod(methodName);
            methodInfo.Should().NotBeNull();
            var generator = new GE04CodeFileGenerator();
            var re = generator.GenerateCode(new GE04CodeFile
            {
                InterfaceName = $"I{methodInfo.Name}",
                EventDataFullName = typeof(TestEventDataType).FullName,
                StateDataFullName = typeof(TestStateDataType).FullName,
                UnwrapTaskReturnTypeName = string.Empty,
                ArgumentTypeAndNames = Enumerable.Empty<string>().ToArray()
            });
            _testOutputHelper.WriteCodePretty(re);

            AssertCodeFile(nameof(TestTaskMethodTest), re);
        }

        [Fact]
        public void IntReturnMethodTest()
        {
            const string methodName = nameof(ITestInterface.IntReturnMethod);
            var methodInfo = typeof(ITestInterface)
                .GetMethod(methodName);
            methodInfo.Should().NotBeNull();
            var generator = new GE04CodeFileGenerator();
            var re = generator.GenerateCode(new GE04CodeFile
            {
                InterfaceName = $"I{methodInfo.Name}",
                EventDataFullName = typeof(TestEventDataType).FullName,
                StateDataFullName = typeof(TestStateDataType).FullName,
                UnwrapTaskReturnTypeName = "int",
                ArgumentTypeAndNames = Enumerable.Empty<string>().ToArray()
            });
            _testOutputHelper.WriteCodePretty(re);
            AssertCodeFile(nameof(IntReturnMethodTest), re);
        }

        [Fact]
        public void ArgumentMethodTest()
        {
            const string methodName = nameof(ITestInterface.ArgumentMethod);
            var methodInfo = typeof(ITestInterface)
                .GetMethod(methodName);
            methodInfo.Should().NotBeNull();
            var generator = new GE04CodeFileGenerator();
            var re = generator.GenerateCode(new GE04CodeFile
            {
                InterfaceName = $"I{methodInfo.Name}",
                EventDataFullName = typeof(TestEventDataType).FullName,
                StateDataFullName = typeof(TestStateDataType).FullName,
                UnwrapTaskReturnTypeName = string.Empty,
                ArgumentTypeAndNames = new[] {"string a", "int b", "TestEventDataType dataType"}
            });
            _testOutputHelper.WriteCodePretty(re);
            AssertCodeFile(nameof(ArgumentMethodTest), re);
        }

        [Fact]
        public void IntReturnArgumentMethodTest()
        {
            const string methodName = nameof(ITestInterface.IntReturnArgumentMethod);
            var methodInfo = typeof(ITestInterface)
                .GetMethod(methodName);
            methodInfo.Should().NotBeNull();
            var generator = new GE04CodeFileGenerator();
            var re = generator.GenerateCode(new GE04CodeFile
            {
                InterfaceName = $"I{methodInfo.Name}",
                EventDataFullName = typeof(TestEventDataType).FullName,
                StateDataFullName = typeof(TestStateDataType).FullName,
                UnwrapTaskReturnTypeName = "int",
                ArgumentTypeAndNames = new[] {"string a", "int b", "TestEventDataType dataType"}
            });
            _testOutputHelper.WriteCodePretty(re);

            AssertCodeFile(nameof(IntReturnArgumentMethodTest), re);
        }
    }
}