using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;


namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE03EventMethodImpl
{
    public class GE03CodeFileGenerator
        : CodeFileGeneratorBase<GE03CodeFileGeneratorContext, GE03CodeFile>
    {
        public override GE03CodeFile CreateCodeFileCore(GE03CodeFileGeneratorContext context)
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
            var className = interfaceName.GetImplName();
            var re = new GE03CodeFile
            {
                ClassName = className,
                InterfaceName = interfaceName,
                ArgumentTypeAndNames = context.MethodDeclarationSyntax.ParameterList.Parameters
                    .Select(x => $"{x.Type} {x.Identifier}").ToArray(),
                EventDataTypeFullName =
                    context.ClaptrapEventMethodMetadata.ClaptrapEventMetadata.EventDataType.FullName,
                StateDataTypeFullName = context.ClaptrapMetadata.StateDataType.FullName,
                UnwrapTaskReturnTypeName = unwrapTaskReturnTypeName,
                FileName = $"{className}.cs",
                Namespaces = SyntaxHelper.GetNamespaces(context.CompilationUnitSyntax).ToArray(),
            };
            return re;
        }

        public override SyntaxTree GenerateCore(GE03CodeFile file)
        {
            var namespaces = file.Namespaces
                .Concat(new[]
                {
                    "System",
                    "Newbe.Claptrap", 
                    "System.Threading.Tasks"
                })
                .Distinct()
                .OrderBy(x => x)
                .ToArray();
            var builder = new StringBuilder();
            foreach (var ns in namespaces)
            {
                builder.AppendLine($"using {ns};");
            }

            builder.AppendLine($@"
using EventData = {file.EventDataTypeFullName};
using StateData = {file.StateDataTypeFullName};");
            builder.AppendLine("namespace Claptrap.N20EventMethods");
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