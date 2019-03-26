using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.StateDataUpdater
{
    public class CodeFileGenerator
        : CodeFileGeneratorBase<CodeFileGeneratorContext, CodeFile>
    {
        public override CodeFile CreateCodeFileCore(CodeFileGeneratorContext context)
        {
            var re = new CodeFile
            {
                StateDataName = context.ClaptrapMetadata.StateDataType.Name,
                EventDataTypeFullName = context.ClaptrapEventMetadata.EventDataType.FullName,
                StateDataTypeFullName = context.ClaptrapMetadata.StateDataType.FullName,
            };
            return re;
        }

        public override SyntaxTree GenerateCore(CodeFile file)
        {
            var builder = new StringBuilder();
            builder.AppendLine($@"using System;
using Newbe.Claptrap;
using StateData = {file.StateDataTypeFullName};
using EventData = {file.EventDataTypeFullName};");
            builder.AppendLine("namespace Claptrap._11StateData" +
                               "Updaters");
            builder.UsingCurlyBraces(() =>
            {
                builder.AppendLine(
                    $"public class {file.StateDataName}Updater : StateDataUpdaterBase<StateData, EventData>");
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