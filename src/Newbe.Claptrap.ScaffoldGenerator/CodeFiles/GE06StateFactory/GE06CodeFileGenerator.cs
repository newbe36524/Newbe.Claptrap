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
            var builder = new StringBuilder();
            builder.AppendLine(@"
using System;
using System.Threading.Tasks;
using Newbe.Claptrap;
using Newbe.Claptrap.Core;");
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