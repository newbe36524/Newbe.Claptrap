using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.EventMethodInterface
{
    public class CodeFileGenerator
        : CodeFileGeneratorBase<CodeFileGeneratorContext, CodeFile>
    {
        public override CodeFile CreateCodeFileCore(CodeFileGeneratorContext context)
        {
            var returnType = context.MethodDeclarationSyntax.ReturnType;
            if (!(returnType is GenericNameSyntax rType) ||
                rType.Identifier.ToString() != "Task")
            {
                throw new ArgumentOutOfRangeException(nameof(context), "method return must be Task or Task<>");
            }

            var re = new CodeFile
            {
                InterfaceName = $"I{context.ClaptrapEventMethodMetadata.MethodInfo.Name}",
                EventDataFullName = context.ClaptrapEventMethodMetadata.ClaptrapEventMetadata.EventDataType.FullName,
                StateDataFullName = context.ClaptrapMetadata.StateDataType.FullName,
                ArgumentTypeAndNames = context.MethodDeclarationSyntax.ParameterList.Parameters
                    .Select(x => $"{x.Type} {x.Identifier}").ToArray(),
                UnwrapTaskReturnTypeName = rType.TypeArgumentList.ToString().RemoveGtLt(),
            };
            return re;
        }

        public override SyntaxTree GenerateCore(CodeFile file)
        {
            var builder = new StringBuilder();
            builder.AppendLine($@"
using Newbe.Claptrap;
using System.Threading.Tasks;
using EventData = {file.EventDataFullName};
using StateData = {file.StateDataFullName};");
            builder.AppendLine("namespace Claptrap._20EventMethods");
            builder.UsingCurlyBraces(() =>
            {
                builder.AppendLine($"public interface {file.InterfaceName}");
                builder.UsingCurlyBraces(() =>
                {
                    builder.Append(string.IsNullOrEmpty(file.UnwrapTaskReturnTypeName)
                        ? "Task<EventMethodResult<EventData>>"
                        : $"Task<EventMethodResult<EventData,{file.UnwrapTaskReturnTypeName}>>");

                    builder.Append(" Invoke(StateData stateData");
                    var parameterInfos = file.ArgumentTypeAndNames;
                    foreach (var parameterInfo in parameterInfos)
                    {
                        builder.Append($", {parameterInfo}");
                    }

                    builder.Append(");");
                });
            });
            var tree = CSharpSyntaxTree.ParseText(builder.ToString());
            return tree;
        }
    }
}