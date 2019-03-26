using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.ClaptrapGrainEventMethodsPart;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.ClaptrapGrainNoneEventMethodPart
{
    public class CodeFileGenerator
        : CodeFileGeneratorBase<CodeFileGeneratorContext, CodeFile>
    {
        public override CodeFile CreateCodeFileCore(CodeFileGeneratorContext context)
        {
            var list = new List<NoneEventMethod>();
            var methodNodes = context.CompilationUnitSyntax
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .ToArray();
            foreach (var eventMethodMetadata in context.ClaptrapMetadata.EventMethodMetadata)
            {
                // using method name to find the code.
                // TODO try to find another way to do this
                var methodNode =
                    methodNodes.Single(x => x.Identifier.ToString() == eventMethodMetadata.MethodInfo.Name);
                var method = new NoneEventMethod
                {
                    MethodName = eventMethodMetadata.MethodInfo.Name,
                    ReturnTypeName = methodNode.ReturnType.ToString(),
                    ArgumentTypeAndNames = methodNode.ParameterList.Parameters
                        .Select(x => $"{x.Type} {x.Identifier.ToString()}").ToArray(),
                };
                list.Add(method);
            }

            var re = new CodeFile
            {
                ClassName = context.ClaptrapMetadata.InterfaceType.Name.GetImplName(),
                NoneEventMethods = list
            };
            return re;
        }

        public override SyntaxTree GenerateCore(CodeFile file)
        {
            var builder = new StringBuilder();
            builder.AppendLine("using System.Threading.Tasks;");
            builder.AppendLine("namespace Domain.Claptrap");
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