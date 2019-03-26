using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public class ScaffoldGenerator : IScaffoldGenerator
    {
        private readonly IClaptrapInterfaceProjectFileProvider _claptrapInterfaceProjectFileProvider;
        private readonly IClaptrapScaffoldGenerator _claptrapScaffoldGenerator;
        private readonly IMinionScaffoldGenerator _minionScaffoldGenerator;

        public ScaffoldGenerator(
            IClaptrapInterfaceProjectFileProvider claptrapInterfaceProjectFileProvider,
            IClaptrapScaffoldGenerator claptrapScaffoldGenerator,
            IMinionScaffoldGenerator minionScaffoldGenerator)
        {
            _claptrapInterfaceProjectFileProvider = claptrapInterfaceProjectFileProvider;
            _claptrapScaffoldGenerator = claptrapScaffoldGenerator;
            _minionScaffoldGenerator = minionScaffoldGenerator;
        }

        public async Task Generate(ScaffoldGenerateContext context)
        {
            // read all source file
            var sourceFileInfos = await Task.WhenAll(_claptrapInterfaceProjectFileProvider.GetAllFiles().Select(
                async x =>
                {
                    var content = await File.ReadAllTextAsync(x.FullName);
                    return new
                    {
                        FileName = x.Name,
                        InterfaceName = x.Name.Replace(".cs", ""),
                        Content = content,
                        CompilationUnitSyntax = CSharpSyntaxTree.ParseText(content).GetCompilationUnitRoot(),
                    };
                }));

            // assume all interface name are different
            var sourceFileInfoDic = sourceFileInfos.ToDictionary(x => x.InterfaceName);

            // generate code for claptrap
            var claptrap = context.ActorMetadataCollection.ClaptrapMetadata.Select(x =>
            {
                if (!sourceFileInfoDic.TryGetValue(x.InterfaceType.Name, out var sourceFileInfo))
                {
                    // TODO custom exception maybe
                    throw new Exception(
                        $"there is no source named {x.InterfaceType.Name}.cs, please check your fullname or interface name in your interface project");
                }

                return new
                {
                    SourceFileInfo = sourceFileInfo,
                    ClaptrapMetadata = x,
                };
            });

            await Task.WhenAll(claptrap.Select(x =>
                _claptrapScaffoldGenerator.Generate(x.ClaptrapMetadata, x.SourceFileInfo.CompilationUnitSyntax)));


            // generate code for minion
            var minion = context.ActorMetadataCollection.MinionMetadata.Select(x =>
            {
                if (!sourceFileInfoDic.TryGetValue(x.InterfaceType.Name, out var sourceFileInfo))
                {
                    // TODO custom exception maybe
                    throw new Exception(
                        $"there is no source named {x.InterfaceType.Name}.cs, please check your fullname or interface name in your interface project");
                }

                return new
                {
                    SourceFileInfo = sourceFileInfo,
                    ClaptrapMetadata = x,
                };
            });

            await Task.WhenAll(minion.Select(x =>
                _minionScaffoldGenerator.Generate(x.ClaptrapMetadata, x.SourceFileInfo.CompilationUnitSyntax)));
        }
    }
}