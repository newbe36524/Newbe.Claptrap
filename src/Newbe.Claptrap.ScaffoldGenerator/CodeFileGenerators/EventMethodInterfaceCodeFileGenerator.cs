using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFileGenerators
{
    public class EventMethodInterfaceCodeFileGenerator : ICodeFileGenerator
    {
        private readonly Type _stateDataType;
        private readonly ClaptrapEventMethodCodeInfo _claptrapEventMethodCodeInfo;

        public EventMethodInterfaceCodeFileGenerator(
            Type stateDataType,
            ClaptrapEventMethodCodeInfo claptrapEventMethodCodeInfo)
        {
            _stateDataType = stateDataType;
            _claptrapEventMethodCodeInfo = claptrapEventMethodCodeInfo;
        }

        public Task<SyntaxTree> Generate()
        {
            var builder = new StringBuilder();
            builder.AppendLine($@"
using Newbe.Claptrap;
using System.Threading.Tasks;
using EventData = {_claptrapEventMethodCodeInfo.Metadata.ClaptrapEventMetadata.EventDataType.FullName};
using StateData = {_stateDataType.FullName};");
            builder.AppendLine("namespace Claptrap._20EventMethods");
            builder.UsingCurlyBraces(() =>
            {
                builder.AppendLine($"public interface {_claptrapEventMethodCodeInfo.InterfaceName}");
                builder.UsingCurlyBraces(() =>
                {
                    if (_claptrapEventMethodCodeInfo.ReturnType == null)
                    {
                        builder.Append("Task<EventMethodResult<EventData>>");
                    }
                    else
                    {
                        builder.Append(
                            $"Task<EventMethodResult<EventData,{_claptrapEventMethodCodeInfo.ReturnType.FullName}>>");
                    }

                    builder.Append(" Invoke(StateData stateData");
                    var parameterInfos = _claptrapEventMethodCodeInfo.ParameterInfos;
                    if (parameterInfos.Length > 0)
                    {
                        builder.Append(",");
                        builder.AppendJoin(",", parameterInfos.Select(x => $"{x.ParameterType} {x.Name}"));
                    }

                    builder.Append(");");
                });
            });
            var tree = CSharpSyntaxTree.ParseText(builder.ToString());
            return Task.FromResult(tree);
        }
    }
}