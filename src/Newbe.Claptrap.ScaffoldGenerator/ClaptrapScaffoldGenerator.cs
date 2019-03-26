using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    using ClaptrapGrainEventMethodsPartCodeFileGeneratorContext =
        CodeFiles.ClaptrapGrainEventMethodsPart.CodeFileGeneratorContext;
    using ClaptrapGrainEventMethodsPartCodeFileGenerator =
        CodeFiles.ClaptrapGrainEventMethodsPart.CodeFileGenerator;

    public class ClaptrapScaffoldGenerator : IClaptrapScaffoldGenerator
    {
        private readonly IScaffoldFileSystem _scaffoldFileSystem;

        public ClaptrapScaffoldGenerator(
            IScaffoldFileSystem scaffoldFileSystem)
        {
            _scaffoldFileSystem = scaffoldFileSystem;
        }

        public async Task Generate(ClaptrapMetadata claptrapMetadata, CompilationUnitSyntax compilationUnitSyntax)
        {
            // 1. generate code
            // 2. save code to file
            ICodeFileGenerator codeFileGenerator = new ClaptrapGrainEventMethodsPartCodeFileGenerator();
            ICodeFile codeFile = codeFileGenerator.CreateCodeFile(
                new ClaptrapGrainEventMethodsPartCodeFileGeneratorContext
                {
                    ClaptrapMetadata = claptrapMetadata,
                    CompilationUnitSyntax = compilationUnitSyntax,
                });
            var generate = codeFileGenerator.Generate(codeFile);
        }
    }
}