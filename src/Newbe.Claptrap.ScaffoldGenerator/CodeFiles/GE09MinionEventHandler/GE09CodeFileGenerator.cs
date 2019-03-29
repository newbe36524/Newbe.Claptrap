using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE09MinionEventHandler
{
    public class GE09CodeFileGenerator
        : CodeFileGeneratorBase<GE09CodeFileGeneratorContext, GE09CodeFile>
    {
        public override GE09CodeFile CreateCodeFileCore(GE09CodeFileGeneratorContext context)
        {
            var className = $"{context.EventType}EventHandler";
            var re = new GE09CodeFile
            {
                ClassName = className,
                EventDataTypeFullName = context.EventDataType.FullName,
                StateDataTypeFullName = context.StateDataType.FullName,
                FileName = $"{className}.cs",
                Namespaces = SyntaxHelper.GetNamespaces(context.CompilationUnitSyntax).ToArray(),
            };
            return re;
        }

        public override SyntaxTree GenerateCore(GE09CodeFile file)
        {
            var builder = new StringBuilder();
            var namespaces = file.Namespaces
                .Concat(new[]
                {
                    "System",
                    "Newbe.Claptrap",
                    "Newbe.Claptrap.Core",
                    "System.Threading.Tasks"
                })
                .Distinct()
                .OrderBy(x => x)
                .ToArray();
            foreach (var ns in namespaces)
            {
                builder.AppendLine($"using {ns};");
            }

            builder.AppendLine($@"
using StateData = {file.StateDataTypeFullName};
using EventData = {file.EventDataTypeFullName};");
            builder.AppendLine("namespace Minion.N30EventHandlers" +
                               "Updaters");
            builder.UsingCurlyBraces(() =>
            {
                builder.AppendLine(
                    $"public class {file.ClassName} : MinionEventHandlerBase<StateData, EventData>");
                builder.UsingCurlyBraces(() =>
                {
                    builder.AppendLine(
                        "public override Task HandleEventCore(StateData stateData, EventData eventData)");
                    builder.UsingCurlyBraces(() => { builder.AppendNotImplemented(); });
                });
            });

            var tree = CSharpSyntaxTree.ParseText(builder.ToString());
            return tree;
        }
    }
}