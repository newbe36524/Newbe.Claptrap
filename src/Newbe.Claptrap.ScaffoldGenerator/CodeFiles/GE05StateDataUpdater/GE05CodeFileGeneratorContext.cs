using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE05StateDataUpdater
{
    public class GE05CodeFileGeneratorContext : ICodeFileGeneratorContext
    {
        public string EventType { get; set; }
        public Type StateDataType { get; set; }
        public Type EventDataType { get; set; }
        public CompilationUnitSyntax CompilationUnitSyntax { get; set; }
    }
}