using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE05StateDataUpdater
{
    public class GE05CodeFileGenerator
        : CodeFileGeneratorBase<GE05CodeFileGeneratorContext, GE05CodeFile>
    {
        public override GE05CodeFile CreateCodeFileCore(GE05CodeFileGeneratorContext context)
        {
            var className = $"{context.EventType}Updater";
            var re = new GE05CodeFile
            {
                ClassName = className,
                EventDataTypeFullName = context.EventDataType.FullName,
                StateDataTypeFullName = context.StateDataType.FullName,
                FileName = $"{className}.cs"
            };
            return re;
        }

        public override SyntaxTree GenerateCore(GE05CodeFile file)
        {
            var builder = new StringBuilder();
            var namespaces = file.Namespaces
                .Concat(new[] {"System;","Newbe.Claptrap;", "System.Threading.Tasks"})
                .Distinct()
                .OrderBy(x => x)
                .ToArray();
            foreach (var ns in namespaces)
            {
                builder.AppendLine($"using {ns}");
            }
            builder.AppendLine($@"
using StateData = {file.StateDataTypeFullName};
using EventData = {file.EventDataTypeFullName};");
            builder.AppendLine("namespace Claptrap.N11StateData" +
                               "Updaters");
            builder.UsingCurlyBraces(() =>
            {
                builder.AppendLine(
                    $"public class {file.ClassName} : StateDataUpdaterBase<StateData, EventData>");
                builder.UsingCurlyBraces(() =>
                {
                    builder.AppendLine(
                        "public override void UpdateState(StateData stateData, EventData eventData)");
                    builder.UsingCurlyBraces(() => { builder.AppendNotImplementedException(); });
                });
            });

            var tree = CSharpSyntaxTree.ParseText(builder.ToString());
            return tree;
        }
    }
}