using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public interface ICodeFileGenerator
    {
        ICodeFile CreateCodeFile(ICodeFileGeneratorContext context);
        SyntaxTree GenerateCode(ICodeFile codeFile);
    }
}