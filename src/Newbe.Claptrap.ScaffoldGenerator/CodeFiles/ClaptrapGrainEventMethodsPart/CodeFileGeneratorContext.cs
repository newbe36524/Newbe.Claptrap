using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.ClaptrapGrainEventMethodsPart
{
    public class CodeFileGeneratorContext : ICodeFileGeneratorContext
    {
        public ClaptrapMetadata ClaptrapMetadata { get; set; }

        /// <summary>
        /// code file of claptrap interface
        /// </summary>
        public CompilationUnitSyntax CompilationUnitSyntax { get; set; }
    }
}