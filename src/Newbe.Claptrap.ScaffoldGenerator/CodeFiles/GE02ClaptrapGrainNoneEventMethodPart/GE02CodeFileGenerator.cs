using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE02ClaptrapGrainNoneEventMethodPart
{
    public class GE02CodeFileGenerator
        : CodeFileGeneratorBase<GE02CodeFileGeneratorContext, GE02CodeFile>
    {
        public override GE02CodeFile CreateCodeFileCore(GE02CodeFileGeneratorContext context)
        {
            var list = new List<NoneEventMethod>();
            var methodNodes = context.CompilationUnitSyntax
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .ToArray();
            foreach (var methodInfo in context.ClaptrapMetadata.NoneEventMethodInfos)
            {
                // using method name to find the code.
                // TODO try to find another way to do this
                var methodNode =
                    methodNodes.Single(x => x.Identifier.ToString() == methodInfo.Name);
                var method = new NoneEventMethod
                {
                    MethodName = methodInfo.Name,
                    ReturnTypeName = methodNode.ReturnType.ToString(),
                    ArgumentTypeAndNames = methodNode.ParameterList.Parameters
                        .Select(x => $"{x.Type} {x.Identifier.ToString()}").ToArray(),
                };
                list.Add(method);
            }

            var className = context.ClaptrapMetadata.InterfaceType.Name.GetImplName();
            var re = new GE02CodeFile
            {
                ClassName = className,
                NoneEventMethods = list,
                FileName = $"{className}.cs",
                Namespaces = SyntaxHelper.GetNamespaces(context.CompilationUnitSyntax).ToArray(),
            };
            return re;
        }

        public override SyntaxTree GenerateCore(GE02CodeFile file)
        {
            var builder = new StringBuilder();
            var namespaces = file.Namespaces
                .Concat(new[]
                {
                    "System",
                    "System.Threading.Tasks",
                })
                .Distinct()
                .OrderBy(x => x)
                .ToArray();
            foreach (var ns in namespaces)
            {
                builder.AppendLine($"using {ns};");
            }
            builder.AppendLine("namespace Claptrap");
            builder.UsingCurlyBraces(() =>
            {
                builder.AppendLine($"public partial class {file.ClassName}");
                builder.UsingCurlyBraces(() =>
                {
                    foreach (var noneEventMethod in file.NoneEventMethods)
                    {
                        builder.AppendLine(
                            $"public {noneEventMethod.ReturnTypeName} {noneEventMethod.MethodName}({string.Join(",", noneEventMethod.ArgumentTypeAndNames)})");
                        builder.UsingCurlyBraces(() => { builder.AppendNotImplemented(); });
                    }
                });
            });

            var tree = CSharpSyntaxTree.ParseText(builder.ToString());
            return tree;
        }
    }
}