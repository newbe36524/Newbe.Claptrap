using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.EventMethodImpl
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
                ClassName = context.ClaptrapEventMethodMetadata.MethodInfo.Name,
                InterfaceName = $"I{context.ClaptrapEventMethodMetadata.MethodInfo.Name}",
                ArgumentTypeAndNames = context.MethodDeclarationSyntax.ParameterList.Parameters
                    .Select(x => $"{x.Type} {x.Identifier}").ToArray(),
                EventDataTypeFullName =
                    context.ClaptrapEventMethodMetadata.ClaptrapEventMetadata.EventDataType.FullName,
                StateDataTypeFullName = context.ClaptrapMetadata.StateDataType.FullName,
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
using EventData = {file.EventDataTypeFullName};
using StateData = {file.StateDataTypeFullName};");
            builder.AppendLine("namespace Claptrap._20EventMethods");
            builder.UsingCurlyBraces(() =>
            {
                builder.AppendLine($"public class {file.ClassName}:{file.InterfaceName}");
                builder.UsingCurlyBraces(() =>
                {
                    builder.Append("public ");
                    if (string.IsNullOrEmpty(file.UnwrapTaskReturnTypeName))
                    {
                        builder.Append("Task<EventMethodResult<EventData>>");
                    }
                    else
                    {
                        builder.Append($"Task<EventMethodResult<EventData,{file.UnwrapTaskReturnTypeName}>>");
                    }

                    builder.Append(" Invoke(StateData stateData");
                    if (file.ArgumentTypeAndNames.Length > 0)
                    {
                        builder.Append(",");
                        builder.AppendJoin(",", file.ArgumentTypeAndNames);
                    }

                    builder.Append(")");
                    builder.UsingCurlyBraces(() => builder.AppendNotImplementedException());
                });
            });
            var tree = CSharpSyntaxTree.ParseText(builder.ToString());
            return tree;
        }
    }
}