using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFileGenerators
{
    public class StateDataUpdaterCodeFileGenerator : ICodeFileGenerator
    {
        private readonly Type _stateDataType;
        private readonly Type _eventDataType;

        public StateDataUpdaterCodeFileGenerator(
            Type stateDataType,
            Type eventDataType)
        {
            _stateDataType = stateDataType;
            _eventDataType = eventDataType;
        }

        public Task<SyntaxTree> Generate()
        {
            var builder = new StringBuilder();
            builder.AppendLine($@"using System;
using Newbe.Claptrap;
using StateData = {_stateDataType.FullName};
using EventData = {_eventDataType.FullName};");
            builder.AppendLine("namespace Claptrap._11StateDataUpdaters");
            builder.UsingCurlyBraces(() =>
            {
                builder.AppendLine(
                    $"public class {_stateDataType.Name}Updater : StateDataUpdaterBase<StateData, EventData>");
                builder.UsingCurlyBraces(() =>
                {
                    builder.AppendLine(
                        "public override void UpdateState(StateData stateData, EventData eventData)");
                    builder.UsingCurlyBraces(() => { builder.AppendNotImplementedException(); });
                });
            });

            var tree = CSharpSyntaxTree.ParseText(builder.ToString());
            return Task.FromResult(tree);
        }
    }
}