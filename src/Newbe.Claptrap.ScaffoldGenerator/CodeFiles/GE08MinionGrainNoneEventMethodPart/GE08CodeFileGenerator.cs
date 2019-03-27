using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE08MinionGrainNoneEventMethodPart
{
    public class GE08CodeFileGenerator
        : CodeFileGeneratorBase<GE08CodeFileGeneratorContext, GE08CodeFile>
    {
        public override GE08CodeFile CreateCodeFileCore(GE08CodeFileGeneratorContext context)
        {
            var list = new List<NoneEventMethod>();
            var methodNodes = context.CompilationUnitSyntax
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .ToArray();
            foreach (var methodInfo in context.MinionMetadata.NoneEventMethodInfos)
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

            var className = context.MinionMetadata.InterfaceType.Name.GetImplName();
            var re = new GE08CodeFile
            {
                ClassName = className,
                NoneEventMethods = list,
                FileName = $"{className}.cs"
            };
            return re;
        }

        public override SyntaxTree GenerateCore(GE08CodeFile file)
        {
            var builder = new StringBuilder();
            builder.AppendLine("using System.Threading.Tasks;");
            builder.AppendLine("namespace Domain.Minion");
            builder.UsingCurlyBraces(() =>
            {
                builder.AppendLine($"public partial class {file.ClassName}");
                builder.UsingCurlyBraces(() =>
                {
                    foreach (var noneEventMethod in file.NoneEventMethods)
                    {
                        builder.AppendLine(
                            $"public {noneEventMethod.ReturnTypeName} {noneEventMethod.MethodName}({string.Join(",", noneEventMethod.ArgumentTypeAndNames)})");
                        builder.UsingCurlyBraces(() => { builder.AppendNotImplementedException(); });
                    }
                });
            });

            var tree = CSharpSyntaxTree.ParseText(builder.ToString());
            return tree;
        }
    }
}