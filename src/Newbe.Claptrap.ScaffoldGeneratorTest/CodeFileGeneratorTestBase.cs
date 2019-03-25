using System.IO;
using Microsoft.CodeAnalysis;

namespace Newbe.Claptrap.ScaffoldGeneratorTest
{
    public abstract class CodeFileGeneratorTestBase
    {
        public void AssertCodeFile(string methodName, SyntaxTree code)
        {
            var methodInfo = GetType().GetMethod(methodName);
            var codeFilename = Path.Combine("CodeFiles", methodInfo.DeclaringType.Name, $"{methodInfo.Name}.cs");
            if (!File.Exists(codeFilename))
            {
                throw new FileNotFoundException($"there should be a code file in {codeFilename}");
            }

            code.ShouldBe(File.ReadAllText(codeFilename));
        }
    }
}