using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Logging;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE01ClaptrapGrainEventMethodsPart
{
    public class GE01CodeFileGenerator
        : CodeFileGeneratorBase<GE01CodeFileGeneratorContext, GE01CodeFile>
    {
        public override GE01CodeFile CreateCodeFileCore(
            GE01CodeFileGeneratorContext context)
        {
            var list = new List<EventMethod>();
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
                var interfaceName =
                    $"N20EventMethods.{eventMethodMetadata.MethodInfo.Name}.I{eventMethodMetadata.MethodInfo.Name}Method";
                var (isSupport, unwrapTaskReturnTypeName) =
                    SyntaxHelper.UnwrapTaskReturnTypeName(methodNode.ReturnType);
                if (!isSupport)
                {
                    throw new ArgumentOutOfRangeException(nameof(context),
                        $"method return type {methodNode.ReturnType} is not supported, method return must be Task or Task<>");

                }
                var method = new EventMethod
                {
                    MethodName = eventMethodMetadata.MethodInfo.Name,
                    ReturnType = methodNode.ReturnType.ToString(),
                    ArgumentNames = methodNode.ParameterList.Parameters.Select(x => x.Identifier.ToString()).ToArray(),
                    ArgumentTypeAndNames = methodNode.ParameterList.Parameters
                        .Select(x => $"{x.Type} {x.Identifier.ToString()}").ToArray(),
                    EventType = eventMethodMetadata.ClaptrapEventMetadata.EventType,
                    EventMethodInterfaceFullName = interfaceName,
                    UnwrapTaskReturnTypeName = unwrapTaskReturnTypeName,
                };
                list.Add(method);
            }

            var className = context.ClaptrapMetadata.InterfaceType.Name.GetImplName();
            var re = new GE01CodeFile
            {
                InterfaceName = context.ClaptrapMetadata.InterfaceType.Name,
                ClassName = className,
                ClaptrapCatalog = context.ClaptrapMetadata.ClaptrapKind.Catalog,
                StateDataTypeFullName = context.ClaptrapMetadata.StateDataType.FullName,
                EventMethods = list,
                FileName = $"{className}.g.cs",
                Namespaces = SyntaxHelper.GetNamespaces(context.CompilationUnitSyntax).ToArray(),
            };

            return re;
        }

        public override SyntaxTree GenerateCore(GE01CodeFile file)
        {
            var builder = new StringBuilder();
            var namespaces = file.Namespaces
                .Concat(new[]
                {
                    "Newbe.Claptrap",
                    "System",
                    "System.Threading.Tasks",
                    "Newbe.Claptrap.Attributes",
                    "Newbe.Claptrap.Core",
                    "Newbe.Claptrap.Orleans",
                    "Orleans"
                })
                .Distinct()
                .OrderBy(x => x)
                .ToArray();
            foreach (var ns in namespaces)
            {
                builder.AppendLine($"using {ns};");
            }

            builder.AppendLine($"using StateData = {file.StateDataTypeFullName};");
            builder.AppendLine("namespace Claptrap");
            builder.UsingCurlyBraces(() =>
            {
                builder.AppendLine($"[ClaptrapComponent(\"{file.ClaptrapCatalog}\")]");
                builder.AppendLine(
                    $" public partial class {file.ClassName} : Grain, {file.InterfaceName}");
                builder.UsingCurlyBraces(() =>
                {
                    builder.AppendLine("public override async Task OnActivateAsync()");
                    builder.UsingCurlyBraces(() =>
                    {
                        builder.AppendLine($@"await base.OnActivateAsync();
                                var kind = new ClaptrapKind(ActorType.Claptrap, ""{file.ClaptrapCatalog}"");
                                var identity = new GrainActorIdentity(kind, this.GetPrimaryKeyString());
                                var factory = (IActorFactory) ServiceProvider.GetService(typeof(IActorFactory));
                                Actor = factory.Create(identity);
                                await Actor.ActivateAsync();");
                    });

                    builder.AppendLine("public override async Task OnDeactivateAsync()");
                    builder.UsingCurlyBraces(() =>
                    {
                        builder.AppendLine(@" await base.OnDeactivateAsync();
            await Actor.DeactivateAsync();");
                    });

                    builder.AppendLine("public IActor Actor { get; private set; }");
                    builder.AppendLine("public StateData ActorState => (StateData) Actor.State.Data;");

                    foreach (var eventMethod in file.EventMethods)
                    {
                        builder.AppendLine(
                            $"public async {eventMethod.ReturnType} {eventMethod.MethodName}({string.Join(",", eventMethod.ArgumentTypeAndNames)})");
                        builder.UsingCurlyBraces(() =>
                        {
                            builder.AppendLine(
                                $"var method = ({eventMethod.EventMethodInterfaceFullName}) ServiceProvider.GetService(typeof({eventMethod.EventMethodInterfaceFullName}));");
                            builder.AppendLine(
                                $"var result = await method.Invoke((StateData) Actor.State.Data{string.Join(" ", eventMethod.ArgumentNames.Select(x => "," + x))});");
                            builder.AppendLine("if (result.EventRaising)");
                            builder.UsingCurlyBraces(() =>
                            {
                                builder.AppendLine(
                                    $@"var @event = new DataEvent(Actor.State.Identity, ""{eventMethod.EventType}"", result.EventData,result.EventUid);");
                                builder.AppendLine("await Actor.HandleEvent(@event);");
                            });
                            if (!string.IsNullOrEmpty(eventMethod.UnwrapTaskReturnTypeName))
                            {
                                builder.AppendLine("return result.MethodReturn;");
                            }
                        });
                    }
                });
            });

            var tree = CSharpSyntaxTree.ParseText(builder.ToString());
            return tree;
        }
    }
}