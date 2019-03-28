using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Newbe.Claptrap.ScaffoldGenerator.Logging;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public class ScaffoldGenerator : IScaffoldGenerator
    {
        private readonly IScaffoldFileSystem _scaffoldFileSystem;
        private readonly IClaptrapInterfaceProjectFileProvider _claptrapInterfaceProjectFileProvider;
        private readonly IClaptrapScaffoldGenerator _claptrapScaffoldGenerator;
        private readonly IMinionScaffoldGenerator _minionScaffoldGenerator;
        private static readonly ILog Logger = LogProvider.For<ScaffoldGenerator>();

        public ScaffoldGenerator(
            IScaffoldFileSystem scaffoldFileSystem,
            IClaptrapInterfaceProjectFileProvider claptrapInterfaceProjectFileProvider,
            IClaptrapScaffoldGenerator claptrapScaffoldGenerator,
            IMinionScaffoldGenerator minionScaffoldGenerator)
        {
            _scaffoldFileSystem = scaffoldFileSystem;
            _claptrapInterfaceProjectFileProvider = claptrapInterfaceProjectFileProvider;
            _claptrapScaffoldGenerator = claptrapScaffoldGenerator;
            _minionScaffoldGenerator = minionScaffoldGenerator;
        }

        public ScaffoldGenerateContext Context { get; set; }

        public async Task Generate()
        {
            // read all source file
            var sourceFileInfos = await Task.WhenAll(_claptrapInterfaceProjectFileProvider.GetAllFiles()
                // get interface files
                .Where(x => x.Name.StartsWith("I"))
                .Select(
                    async x =>
                    {
                        var content = await File.ReadAllTextAsync(x.FullName);
                        return new
                        {
                            FileName = x.Name,
                            FullName = x.FullName,
                            InterfaceName = x.Name.Replace(".cs", ""),
                            Content = content,
                            CompilationUnitSyntax = CSharpSyntaxTree.ParseText(content).GetCompilationUnitRoot(),
                        };
                    }));

            Logger.Trace("there are {count} source files found.", sourceFileInfos.Length);

            // remove all files
            await _scaffoldFileSystem.RemoveAll();

            // assume all interface name are different
            var sourceFileInfoDic = sourceFileInfos.ToDictionary(x => x.InterfaceName);

            // generate code for claptrap
            var claptrap = Context.ActorMetadataCollection.ClaptrapMetadata.Select(x =>
            {
                if (!sourceFileInfoDic.TryGetValue(x.InterfaceType.Name, out var sourceFileInfo))
                {
                    // TODO custom exception maybe
                    throw new Exception(
                        $"there is no source named {x.InterfaceType.Name}.cs, please check your filename or interface name in your interface project");
                }

                return new
                {
                    SourceFileInfo = sourceFileInfo,
                    ClaptrapMetadata = x,
                };
            });

            await Task.WhenAll(claptrap.Select(x =>
                _claptrapScaffoldGenerator.Generate(new ClaptrapScaffoldGeneratorContext
                {
                    ClaptrapMetadata = x.ClaptrapMetadata,
                    CompilationUnitSyntax = x.SourceFileInfo.CompilationUnitSyntax,
                    IsDomainService = x.SourceFileInfo.FullName.Contains("DomainService")
                })));


            // generate code for minion
            var minion = Context.ActorMetadataCollection.MinionMetadata.Select(x =>
            {
                if (!sourceFileInfoDic.TryGetValue(x.InterfaceType.Name, out var sourceFileInfo))
                {
                    // TODO custom exception maybe
                    throw new Exception(
                        $"there is no source named {x.InterfaceType.Name}.cs, please check your filename or interface name in your interface project");
                }

                return new
                {
                    SourceFileInfo = sourceFileInfo,
                    MinionMetadata = x,
                };
            });

            await Task.WhenAll(minion.Select(x =>
                _minionScaffoldGenerator.Generate(new MinionScaffoldGeneratorContext
                {
                    MinionMetadata = x.MinionMetadata,
                    CompilationUnitSyntax = x.SourceFileInfo.CompilationUnitSyntax,
                    IsDomainService = x.SourceFileInfo.FullName.Contains("DomainService")
                })));
        }
    }
}