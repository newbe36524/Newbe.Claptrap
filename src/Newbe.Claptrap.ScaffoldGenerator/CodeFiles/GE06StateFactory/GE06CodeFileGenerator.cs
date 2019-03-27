using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE06StateFactory
{
    public class GE06CodeFileGenerator
        : CodeFileGeneratorBase<GE06CodeFileGeneratorContext, GE06CodeFile>
    {
        public override GE06CodeFile CreateCodeFileCore(GE06CodeFileGeneratorContext context)
        {
            var re = new GE06CodeFile
            {
                StateDataTypeFullName = context.StateDataType.FullName,
                FileName = "StateDataFactory.cs"
            };
            return re;
        }

        public override SyntaxTree GenerateCore(GE06CodeFile file)
        {
            var namespaces = file.Namespaces
                .Concat(new[] {"System;","Newbe.Claptrap;","Newbe.Claptrap.Core;","Newbe.Claptrap;", "System.Threading.Tasks;"})
                .Distinct()
                .OrderBy(x => x)
                .ToArray();
            var builder = new StringBuilder();
            foreach (var ns in namespaces)
            {
                builder.AppendLine($"using {ns}");
            }
            builder.AppendLine($"using StateData = {file.StateDataTypeFullName};");
            builder.AppendLine($"namespace Claptrap._10StateDataFactory");
            builder.UsingCurlyBraces(() =>
            {
                builder.AppendLine($"public class StateDataFactory: StateDataFactoryBase<StateData>");
                builder.UsingCurlyBraces(() =>
                {
                    builder.AppendLine(
                        "public StateDataFactory(IActorIdentity actorIdentity) : base(actorIdentity)");
                    builder.UsingCurlyBraces(() => { });

                    builder.AppendLine("public override Task<StateData> Create()");
                    builder.UsingCurlyBraces(() => { builder.AppendNotImplementedException(); });
                });
            });

            var tree = CSharpSyntaxTree.ParseText(builder.ToString());
            return tree;
        }
    }
}