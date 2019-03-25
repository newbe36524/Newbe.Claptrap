using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.ScaffoldGenerator.CodeFileGenerators;
using Xunit;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public class ClaptrapGrainNoneEventMethodPartCodeFileGeneratorTests
        : CodeFileGeneratorTestBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ClaptrapGrainNoneEventMethodPartCodeFileGeneratorTests(
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task NoneEventMethodWithoutReturnValue()
        {
            var claptrapGrainNoneEventMethodPartCodeFileGenerator =
                new ClaptrapGrainNoneEventMethodPartCodeFileGenerator(new ClaptrapMetadata
                {
                    InterfaceType = typeof(ITestClaptrap),
                    NoneEventMethodInfos = typeof(ITestClaptrap).GetMethods()
                });
            var re = await claptrapGrainNoneEventMethodPartCodeFileGenerator.Generate();
            _testOutputHelper.WriteCodePretty(re);
            AssertCodeFile(nameof(NoneEventMethodWithoutReturnValue), re);
        }
    }
}