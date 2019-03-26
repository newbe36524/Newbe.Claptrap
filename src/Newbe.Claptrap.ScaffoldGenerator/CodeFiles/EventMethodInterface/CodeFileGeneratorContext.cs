using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.EventMethodInterface
{
    public class CodeFileGeneratorContext : ICodeFileGeneratorContext
    {
        public ClaptrapEventMethodMetadata ClaptrapEventMethodMetadata { get; set; }
        public ClaptrapMetadata ClaptrapMetadata { get; set; }
        public MethodDeclarationSyntax MethodDeclarationSyntax { get; set; }
    }
}