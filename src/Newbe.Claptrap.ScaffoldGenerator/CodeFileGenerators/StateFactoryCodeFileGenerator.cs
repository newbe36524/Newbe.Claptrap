using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFileGenerators
{
    public class StateFactoryCodeFileGenerator : ICodeFileGenerator
    {
        private readonly Type _stateDataType;

        public StateFactoryCodeFileGenerator(
            Type stateDataType)
        {
            _stateDataType = stateDataType;
        }

        public Task<SyntaxTree> Generate()
        {
            var builder = new StringBuilder();
            builder.AppendLine(@"
using System;
using System.Threading.Tasks;
using Newbe.Claptrap;
using Newbe.Claptrap.Core;");
            builder.AppendLine($"using StateData = {_stateDataType.FullName};");
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
            return Task.FromResult(tree);
        }
    }
}