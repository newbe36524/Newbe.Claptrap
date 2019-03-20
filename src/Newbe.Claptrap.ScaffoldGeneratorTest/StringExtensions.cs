using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public static class StringExtensions
    {
        public static void ShouldBe(this SyntaxTree tree, string code)
        {
            var expected = CSharpSyntaxTree.ParseText(code);
            tree.IsEquivalentTo(expected).Should().BeTrue();
        }
    }
}