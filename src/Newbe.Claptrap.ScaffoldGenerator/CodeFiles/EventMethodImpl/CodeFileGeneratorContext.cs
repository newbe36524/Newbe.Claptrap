using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.EventMethodImpl
{
    public class CodeFileGeneratorContext : ICodeFileGeneratorContext
    {
        public ClaptrapMetadata ClaptrapMetadata { get; set; }
        public ClaptrapEventMethodMetadata ClaptrapEventMethodMetadata { get; set; }
        public MethodDeclarationSyntax MethodDeclarationSyntax { get; set; }
    }
}