using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE05StateDataUpdater;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE06StateFactory;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE07MinionGrainEventMethodsPart;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE08MinionGrainNoneEventMethodPart;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public class MinionScaffoldGenerator : IMinionScaffoldGenerator
    {
        private readonly IScaffoldFileSystem _scaffoldFileSystem;

        public MinionScaffoldGenerator(
            IScaffoldFileSystem scaffoldFileSystem)
        {
            _scaffoldFileSystem = scaffoldFileSystem;
        }

        public Task Generate(MinionScaffoldGeneratorContext context)
        {
            var minionMetadata = context.MinionMetadata;
            var compilationUnitSyntax = context.CompilationUnitSyntax;
            var rootDirectoryName = context.IsDomainService ? "DomainService" : "Domain";
            return Task.WhenAll(RunAll());

            IEnumerable<Task> RunAll()
            {
                if (minionMetadata.StateDataType != typeof(NoneStateData))
                {
                    yield return GE05();
                    yield return GE06();
                }

                yield return GE07();
                yield return GE08();
            }

            Task GE05()
            {
                return Task.WhenAll(RunForEvent(minionMetadata.ClaptrapEventMetadata));

                IEnumerable<Task> RunForEvent(IEnumerable<ClaptrapEventMetadata> eventMethodMetadata)
                {
                    foreach (var claptrapEventMethodMetadata in eventMethodMetadata)
                    {
                        ICodeFileGenerator codeFileGenerator = new GE05CodeFileGenerator();
                        var codeFile = codeFileGenerator.CreateCodeFile(
                            new GE05CodeFileGeneratorContext
                            {
                                EventType = claptrapEventMethodMetadata.EventType,
                                EventDataType = claptrapEventMethodMetadata.EventDataType,
                                StateDataType = minionMetadata.StateDataType,
                                CompilationUnitSyntax = compilationUnitSyntax,
                            });
                        var generate = codeFileGenerator.GenerateCode(codeFile);
                        var formatCode = CodeFormatter.FormatCode(generate);
                        yield return _scaffoldFileSystem.SaveFile(
                            $"{rootDirectoryName}/{minionMetadata.ClaptrapMetadata.ClaptrapKind.Catalog}/Minion/{minionMetadata.MinionKind.MinionCatalog}/N11StateDataUpdaters/{codeFile.FileName}",
                            formatCode);
                    }
                }
            }

            Task GE06()
            {
                ICodeFileGenerator codeFileGenerator = new GE06CodeFileGenerator();
                var codeFile = codeFileGenerator.CreateCodeFile(
                    new GE06CodeFileGeneratorContext
                    {
                        StateDataType = minionMetadata.StateDataType,
                        CompilationUnitSyntax = compilationUnitSyntax,
                    });
                var generate = codeFileGenerator.GenerateCode(codeFile);
                var formatCode = CodeFormatter.FormatCode(generate);
                return _scaffoldFileSystem.SaveFile(
                    $"{rootDirectoryName}/{minionMetadata.ClaptrapMetadata.ClaptrapKind.Catalog}/Minion/{minionMetadata.MinionKind.MinionCatalog}/N10StateDataFactory/{codeFile.FileName}",
                    formatCode);
            }

            Task GE07()
            {
                ICodeFileGenerator codeFileGenerator = new GE07CodeFileGenerator();
                var codeFile = codeFileGenerator.CreateCodeFile(
                    new GE07CodeFileGeneratorContext
                    {
                        MinionMetadata = minionMetadata,
                        CompilationUnitSyntax = compilationUnitSyntax,
                    });
                var generate = codeFileGenerator.GenerateCode(codeFile);
                var formatCode = CodeFormatter.FormatCode(generate);
                return _scaffoldFileSystem.SaveFile(
                    $"{rootDirectoryName}/{minionMetadata.ClaptrapMetadata.ClaptrapKind.Catalog}/Minion/{minionMetadata.MinionKind.MinionCatalog}/{codeFile.FileName}",
                    formatCode);
            }

            Task GE08()
            {
                ICodeFileGenerator codeFileGenerator = new GE08CodeFileGenerator();
                var codeFile = codeFileGenerator.CreateCodeFile(
                    new GE08CodeFileGeneratorContext
                    {
                        CompilationUnitSyntax = compilationUnitSyntax,
                        MinionMetadata = minionMetadata,
                    });
                var generate = codeFileGenerator.GenerateCode(codeFile);
                var formatCode = CodeFormatter.FormatCode(generate);
                return _scaffoldFileSystem.SaveFile(
                    $"{rootDirectoryName}/{minionMetadata.ClaptrapMetadata.ClaptrapKind.Catalog}/Minion/{minionMetadata.MinionKind.MinionCatalog}/{codeFile.FileName}",
                    formatCode);
            }
        }
    }
}