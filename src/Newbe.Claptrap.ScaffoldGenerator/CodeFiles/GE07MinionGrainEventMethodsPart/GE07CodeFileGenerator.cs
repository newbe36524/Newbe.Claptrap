using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE07MinionGrainEventMethodsPart
{
    public class GE07CodeFileGenerator
        : CodeFileGeneratorBase<GE07CodeFileGeneratorContext, GE07CodeFile>
    {
        public override GE07CodeFile CreateCodeFileCore(
            GE07CodeFileGeneratorContext context)
        {
            var className = context.MinionMetadata.InterfaceType.Name.GetImplName();
            var re = new GE07CodeFile
            {
                InterfaceName = context.MinionMetadata.InterfaceType.Name,
                ClassName = className,
                ClaptrapCatalog = context.MinionMetadata.ClaptrapMetadata.ClaptrapKind.Catalog,
                MinionCatalog = context.MinionMetadata.MinionKind.MinionCatalog,
                StateDataTypeFullName = context.MinionMetadata.StateDataType.FullName,
                EventMethods = context.MinionMetadata.MinionEventMethodMetadata.Select(x => x.MethodInfo.Name)
                    .ToArray(),
                FileName = $"{className}.g.cs",
            };

            return re;
        }

        public override SyntaxTree GenerateCore(GE07CodeFile file)
        {
            var namespaces = file.Namespaces
                .Concat(new[]
                {
                    "System;", "Newbe.Claptrap;", "Newbe.Claptrap.Core;", "Newbe.Claptrap;",
                    "Newbe.Claptrap.Attributes;", "System.Threading.Tasks;", "Newbe.Claptrap.Orleans;", "Orleans;"
                })
                .Distinct()
                .OrderBy(x => x)
                .ToArray();
            var builder = new StringBuilder();
            foreach (var ns in namespaces)
            {
                builder.AppendLine($"using {ns}");
            }

            builder.AppendLine($"using StateData = {file.StateDataTypeFullName};");
            builder.AppendLine("namespace Minion");
            builder.UsingCurlyBraces(() =>
            {
                builder.AppendLine($"[MinionComponent(\"{file.ClaptrapCatalog}\",\"{file.MinionCatalog}\")]");
                builder.AppendLine(
                    $" public partial class {file.ClassName} : Grain, {file.InterfaceName}");
                builder.UsingCurlyBraces(() =>
                {
                    builder.AppendLine("public override async Task OnActivateAsync()");
                    builder.UsingCurlyBraces(() =>
                    {
                        builder.AppendLine($@"await base.OnActivateAsync();
                        var actorFactory = (IActorFactory) ServiceProvider.GetService(typeof(IActorFactory));
                        var identity =
                            new GrainActorIdentity(new MinionKind(ActorType.Minion, ""{file.ClaptrapCatalog}"", ""{file.MinionCatalog}""),
                        this.GetPrimaryKeyString());
                        Actor = actorFactory.Create(identity);
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
                        builder.AppendLine($"public Task {eventMethod}(IEvent @event)");
                        builder.UsingCurlyBraces(() => { builder.AppendLine("return Actor.HandleEvent(@event);"); });
                    }
                });
            });

            var tree = CSharpSyntaxTree.ParseText(builder.ToString());
            return tree;
        }
    }
}