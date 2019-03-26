using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.StateFactory
{
    public class CodeFileGenerator
        : CodeFileGeneratorBase<CodeFileGeneratorContext, CodeFile>
    {
        public override CodeFile CreateCodeFileCore(CodeFileGeneratorContext context)
        {
            var re = new CodeFile
            {
                StateDataTypeFullName = context.ClaptrapMetadata.StateDataType.FullName
            };
            return re;
        }

        public override SyntaxTree GenerateCore(CodeFile file)
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