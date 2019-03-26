using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.ClaptrapGrainEventMethodsPart
{
    public class CodeFileGenerator
        : CodeFileGeneratorBase<CodeFileGeneratorContext, CodeFile>
    {
        public override CodeFile CreateCodeFileCore(
            CodeFileGeneratorContext context)
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
                var method = new EventMethod
                {
                    MethodName = eventMethodMetadata.MethodInfo.Name,
                    ReturnType = methodNode.ReturnType.ToString(),
                    ArgumentNames = methodNode.ParameterList.Parameters.Select(x => x.Identifier.ToString()).ToArray(),
                    ArgumentTypeAndNames = methodNode.ParameterList.Parameters
                        .Select(x => $"{x.Type} {x.Identifier.ToString()}").ToArray(),
                    EventType = eventMethodMetadata.ClaptrapEventMetadata.EventType,
                    EventMethodInterfaceName = $"I{eventMethodMetadata.MethodInfo.Name}",
                };
                list.Add(method);
            }

            var re = new CodeFile
            {
                InterfaceName = context.ClaptrapMetadata.InterfaceType.Name,
                ClassName = context.ClaptrapMetadata.InterfaceType.Name.GetImplName(),
                ClaptrapCatalog = context.ClaptrapMetadata.ClaptrapKind.Catalog,
                StateDataTypeFullName = context.ClaptrapMetadata.StateDataType.FullName,
                EventMethods = list
            };

            return re;
        }

        public override SyntaxTree GenerateCore(CodeFile file)
        {
            var builder = new StringBuilder();
            builder.AppendLine($@"using System.Threading.Tasks;
using Newbe.Claptrap;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Orleans;
using Orleans;
using StateData = {file.StateDataTypeFullName};");
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
                                $"var method = ({eventMethod.EventMethodInterfaceName}) ServiceProvider.GetService(typeof({eventMethod.EventMethodInterfaceName}));");
                            builder.AppendLine(
                                $"var result = await method.Invoke((StateData) Actor.State.Data{string.Join(" ", eventMethod.ArgumentNames)});");
                            builder.AppendLine("if (result.EventRaising)");
                            builder.UsingCurlyBraces(() =>
                            {
                                builder.AppendLine(
                                    $@" var @event = new DataEvent(Actor.State.Identity, ""{eventMethod.EventType}"", result.EventData,result.EventUid);");
                                builder.AppendLine("await Actor.HandleEvent(@event);");
                            });
                        });
                    }
                });
            });

            var tree = CSharpSyntaxTree.ParseText(builder.ToString());
            return tree;
        }
    }
}