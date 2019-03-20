using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFileGenerators
{
    public class EventMethodImplCodeFileGenerator : ICodeFileGenerator
    {
        private readonly Type _stateDataType;
        private readonly Type _eventDataType;
        private readonly MethodInfo _methodInfo;

        public EventMethodImplCodeFileGenerator(
            Type stateDataType,
            Type eventDataType,
            MethodInfo methodInfo)
        {
            _stateDataType = stateDataType;
            _eventDataType = eventDataType;
            _methodInfo = methodInfo;
        }

        public Task<SyntaxTree> Generate()
        {
            var builder = new StringBuilder();
            builder.AppendLine($@"
using Newbe.Claptrap;
using System.Threading.Tasks;
using EventData = {_eventDataType.FullName};
using StateData = {_stateDataType.FullName};");
            builder.AppendLine("namespace Claptrap._20EventMethods");
            builder.UsingCurlyBraces(() =>
            {
                builder.AppendLine($"public class {_methodInfo.Name}:I{_methodInfo.Name}");
                builder.UsingCurlyBraces(() =>
                {
                    builder.Append("public ");
                    if (_methodInfo.ReturnType == typeof(Task))
                    {
                        builder.Append("Task<EventMethodResult<EventData>>");
                    }
                    else if (_methodInfo.ReturnType.IsConstructedGenericType &&
                             _methodInfo.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                    {
                        var returnType = _methodInfo.ReturnType.GenericTypeArguments[0];
                        builder.Append($"Task<EventMethodResult<EventData,{returnType.FullName}>>");
                    }
                    else
                    {
                        throw new NotSupportedException("method must return Task or Task<>");
                    }

                    builder.Append(" Invoke(StateData stateData");
                    var parameterInfos = _methodInfo.GetParameters();
                    if (parameterInfos.Length > 0)
                    {
                        builder.Append(",");
                        builder.AppendJoin(",", parameterInfos.Select(x => $"{x.ParameterType} {x.Name}"));
                    }

                    builder.Append(")");
                    builder.UsingCurlyBraces(() => builder.AppendNotImplementedException());
                });
            });
            var tree = CSharpSyntaxTree.ParseText(builder.ToString());
            return Task.FromResult(tree);
        }
    }
}