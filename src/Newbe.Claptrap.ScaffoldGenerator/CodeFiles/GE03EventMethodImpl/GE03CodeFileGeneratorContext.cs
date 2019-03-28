using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE03EventMethodImpl
{
    public class GE03CodeFileGeneratorContext : ICodeFileGeneratorContext
    {
        public CompilationUnitSyntax CompilationUnitSyntax { get; set; }
        public ClaptrapMetadata ClaptrapMetadata { get; set; }
        public ClaptrapEventMethodMetadata ClaptrapEventMethodMetadata { get; set; }
        public MethodDeclarationSyntax MethodDeclarationSyntax { get; set; }
    }
}