using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE06StateFactory
{
    public class GE06CodeFileGeneratorContext : ICodeFileGeneratorContext
    {
        public Type StateDataType { get; set; }
        public CompilationUnitSyntax CompilationUnitSyntax { get; set; }
    }
}