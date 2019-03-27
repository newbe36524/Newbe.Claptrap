using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE04EventMethodInterface
{
    public class GE04CodeFileGeneratorContext : ICodeFileGeneratorContext
    {
        public ClaptrapEventMethodMetadata ClaptrapEventMethodMetadata { get; set; }
        public ClaptrapMetadata ClaptrapMetadata { get; set; }
        public MethodDeclarationSyntax MethodDeclarationSyntax { get; set; }
    }
}