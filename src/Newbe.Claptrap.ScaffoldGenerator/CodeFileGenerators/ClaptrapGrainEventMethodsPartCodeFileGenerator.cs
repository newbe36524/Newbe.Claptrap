using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFileGenerators
{
    public class ClaptrapGrainEventMethodsPartCodeFileGenerator : ICodeFileGenerator
    {
        private readonly ClaptrapMetadata _metadata;

        public ClaptrapGrainEventMethodsPartCodeFileGenerator(
            ClaptrapMetadata metadata)
        {
            _metadata = metadata;
        }

        public Task<SyntaxTree> Generate()
        {
            var builder = new StringBuilder();
            builder.AppendLine($@"using System.Threading.Tasks;
using Newbe.Claptrap;
using Newbe.Claptrap.Attributes;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Orleans;
using Orleans;
using StateData = {_metadata.StateDataType.FullName};");
            builder.AppendLine("namespace Claptrap");
            builder.UsingCurlyBraces(() =>
            {
                builder.AppendLine($"[ClaptrapComponent(\"{_metadata.ClaptrapKind.Catalog}\")]");
                builder.AppendLine(
                    $" public partial class {_metadata.InterfaceType.Name.GetImplName()} : Grain, {_metadata.InterfaceType}");
                builder.UsingCurlyBraces(() =>
                {
                    builder.AppendLine("public override async Task OnActivateAsync()");
                    builder.UsingCurlyBraces(() =>
                    {
                        builder.AppendLine($@"await base.OnActivateAsync();
                                var kind = new ClaptrapKind(ActorType.Claptrap, ""{_metadata.ClaptrapKind.Catalog}"");
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
                    builder.AppendLine($"public StateData ActorState => (StateData) Actor.State.Data;");

                    foreach (var eventMethodMetadata in _metadata.EventMethodMetadata)
                    {
                        var claptrapEventMethodCodeInfo = new ClaptrapEventMethodCodeInfo(eventMethodMetadata);
                        var methodInfo = claptrapEventMethodCodeInfo.Metadata.MethodInfo;
                        builder.AppendLine(
                            $"public async {methodInfo.ReturnType} {methodInfo.Name}({string.Join(",", methodInfo.GetParameters().Select(x => $"{x.ParameterType} {x.Name}"))})");
                        builder.UsingCurlyBraces(() =>
                        {
                            builder.AppendLine(
                                $"var method = ({claptrapEventMethodCodeInfo.InterfaceName}) ServiceProvider.GetService(typeof({claptrapEventMethodCodeInfo.InterfaceName}));");
                            builder.AppendLine(
                                $"var result = await method.Invoke((StateData) Actor.State.Data{string.Join(" ", claptrapEventMethodCodeInfo.ParameterInfos.Select(x => x.Name))});");
                            builder.AppendLine("if (result.EventRaising)");
                            builder.UsingCurlyBraces(() =>
                            {
                                builder.AppendLine(
                                    $" var @event = new DataEvent(Actor.State.Identity, nameof({eventMethodMetadata.ClaptrapEventMetadata.EventDataType.FullName}), result.EventData,result.EventUid);");
                                builder.AppendLine("await Actor.HandleEvent(@event);");
                            });
                        });
                    }
                });
            });

            var tree = CSharpSyntaxTree.ParseText(builder.ToString());
            return Task.FromResult(tree);
        }
    }
}