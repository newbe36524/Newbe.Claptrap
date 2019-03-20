using Microsoft.CodeAnalysis;
using Newbe.Claptrap.ScaffoldGenerator;
using Xunit.Abstractions;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public static class TestOutputHelperExtensions
    {
        public static void WriteCodePretty(this ITestOutputHelper testOutputHelper, SyntaxTree syntaxTree)
        {
            testOutputHelper.WriteLine(CodeFormatter.FormatCode(syntaxTree));
        }
    }
}