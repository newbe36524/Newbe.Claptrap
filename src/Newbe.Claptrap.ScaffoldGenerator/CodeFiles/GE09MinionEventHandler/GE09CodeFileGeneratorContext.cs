using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE09MinionEventHandler
{
    public class GE09CodeFileGeneratorContext : ICodeFileGeneratorContext
    {
        public string EventType { get; set; }
        public Type StateDataType { get; set; }
        public Type EventDataType { get; set; }
        public CompilationUnitSyntax CompilationUnitSyntax { get; set; }
    }
}