using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFileGenerators
{
    public class ClaptrapGrainNoneEventMethodPartCodeFileGenerator
        : ICodeFileGenerator
    {
        private readonly ClaptrapMetadata _claptrapMetadata;

        public ClaptrapGrainNoneEventMethodPartCodeFileGenerator(
            ClaptrapMetadata claptrapMetadata)
        {
            _claptrapMetadata = claptrapMetadata;
        }

        public Task<SyntaxTree> Generate()
        {
            var builder = new StringBuilder();
            builder.AppendLine("using System.Threading.Tasks;");
            builder.AppendLine("namespace Domain.Claptrap");
            builder.UsingCurlyBraces(() =>
            {
                builder.AppendLine($"public partial class {_claptrapMetadata.InterfaceType.Name.GetImplName()}");
                builder.UsingCurlyBraces(() =>
                {
                    foreach (var methodInfo in _claptrapMetadata.NoneEventMethodInfos)
                    {
                        builder.AppendLine(
                            $"public {methodInfo.ReturnType.Name} {methodInfo.Name}({string.Join(",", methodInfo.GetParameters().Select(x => $"{x.ParameterType.Name} {x.Name}"))})");
                        builder.UsingCurlyBraces(() => { builder.AppendNotImplementedException(); });
                    }
                });
            });
            
            var tree = CSharpSyntaxTree.ParseText(builder.ToString());
            return Task.FromResult(tree);
        }
    }
}