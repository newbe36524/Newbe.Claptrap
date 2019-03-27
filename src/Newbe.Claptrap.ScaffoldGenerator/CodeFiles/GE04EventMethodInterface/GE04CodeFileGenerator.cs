using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE04EventMethodInterface
{
    public class GE04CodeFileGenerator
        : CodeFileGeneratorBase<GE04CodeFileGeneratorContext, GE04CodeFile>
    {
        public override GE04CodeFile CreateCodeFileCore(GE04CodeFileGeneratorContext context)
        {
            var returnType = context.MethodDeclarationSyntax.ReturnType;
            var (isSupported, unwrapTaskReturnTypeName) =
                SyntaxHelper.UnwrapTaskReturnTypeName(returnType);
            if (!isSupported)
            {
                throw new ArgumentOutOfRangeException(nameof(context),
                    $"method return type {returnType} is not supported, method return must be Task or Task<>");
            }

            var interfaceName = $"I{context.ClaptrapEventMethodMetadata.MethodInfo.Name}Method";
            var re = new GE04CodeFile
            {
                InterfaceName = interfaceName,
                EventDataFullName = context.ClaptrapEventMethodMetadata.ClaptrapEventMetadata.EventDataType.FullName,
                StateDataFullName = context.ClaptrapMetadata.StateDataType.FullName,
                ArgumentTypeAndNames = context.MethodDeclarationSyntax.ParameterList.Parameters
                    .Select(x => $"{x.Type} {x.Identifier}").ToArray(),
                UnwrapTaskReturnTypeName = unwrapTaskReturnTypeName,
                FileName = $"{interfaceName}.cs"
            };
            return re;
        }

        public override SyntaxTree GenerateCore(GE04CodeFile file)
        {
            var builder = new StringBuilder();
            var namespaces = file.Namespaces
                .Concat(new[] {"Newbe.Claptrap;", "System.Threading.Tasks;"})
                .Distinct()
                .OrderBy(x => x)
                .ToArray();
            foreach (var ns in namespaces)
            {
                builder.AppendLine($"using {ns}");
            }
            builder.AppendLine($@"
using EventData = {file.EventDataFullName};
using StateData = {file.StateDataFullName};");
            builder.AppendLine("namespace Claptrap.N20EventMethods");
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