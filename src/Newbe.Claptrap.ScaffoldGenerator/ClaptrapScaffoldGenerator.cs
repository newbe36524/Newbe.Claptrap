using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE01ClaptrapGrainEventMethodsPart;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE02ClaptrapGrainNoneEventMethodPart;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE03EventMethodImpl;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE04EventMethodInterface;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE05StateDataUpdater;
using Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE06StateFactory;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public class ClaptrapScaffoldGenerator : IClaptrapScaffoldGenerator
    {
        private readonly IScaffoldFileSystem _scaffoldFileSystem;

        public ClaptrapScaffoldGenerator(
            IScaffoldFileSystem scaffoldFileSystem)
        {
            _scaffoldFileSystem = scaffoldFileSystem;
        }

        public Task Generate(ClaptrapScaffoldGeneratorContext context)
        {
            var claptrapMetadata = context.ClaptrapMetadata;
            var compilationUnitSyntax = context.CompilationUnitSyntax;
            var rootDirectoryName = context.IsDomainService ? "DomainService" : "Domain";
            return Task.WhenAll(RunAll());

            IEnumerable<Task> RunAll()
            {
                yield return GE01();
                yield return GE02();
                yield return GE03();
                yield return GE04();
                if (claptrapMetadata.StateDataType != typeof(NoneStateData))
                {
                    yield return GE05();
                    yield return GE06();
                }
            }

            Task GE01()
            {
                ICodeFileGenerator codeFileGenerator = new GE01CodeFileGenerator();
                var codeFile = codeFileGenerator.CreateCodeFile(
                    new GE01CodeFileGeneratorContext
                    {
                        ClaptrapMetadata = claptrapMetadata,
                        CompilationUnitSyntax = compilationUnitSyntax,
                    });
                var generate = codeFileGenerator.GenerateCode(codeFile);
                var formatCode = CodeFormatter.FormatCode(generate);
                return _scaffoldFileSystem.SaveFile(
                    $"{rootDirectoryName}/{claptrapMetadata.ClaptrapKind.Catalog}/Claptrap/{codeFile.FileName}",
                    formatCode);
            }

            Task GE02()
            {
                ICodeFileGenerator codeFileGenerator = new GE02CodeFileGenerator();
                var codeFile = codeFileGenerator.CreateCodeFile(
                    new GE02CodeFileGeneratorContext
                    {
                        ClaptrapMetadata = claptrapMetadata,
                        CompilationUnitSyntax = compilationUnitSyntax,
                    });
                var generate = codeFileGenerator.GenerateCode(codeFile);
                var formatCode = CodeFormatter.FormatCode(generate);
                return _scaffoldFileSystem.SaveFile(
                    $"{rootDirectoryName}/{claptrapMetadata.ClaptrapKind.Catalog}/Claptrap/{codeFile.FileName}",
                    formatCode);
            }

            Task GE03()
            {
                var methodNodes = compilationUnitSyntax
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .ToArray();

                return Task.WhenAll(RunForEvent(claptrapMetadata.EventMethodMetadata));

                IEnumerable<Task> RunForEvent(IEnumerable<ClaptrapEventMethodMetadata> eventMethodMetadata)
                {
                    foreach (var claptrapEventMethodMetadata in eventMethodMetadata)
                    {
                        var methodNode = methodNodes.Single(x =>
                            x.Identifier.ToString() == claptrapEventMethodMetadata.MethodInfo.Name);
                        ICodeFileGenerator codeFileGenerator = new GE03CodeFileGenerator();
                        var codeFile = codeFileGenerator.CreateCodeFile(
                            new GE03CodeFileGeneratorContext
                            {
                                ClaptrapMetadata = claptrapMetadata,
                                ClaptrapEventMethodMetadata = claptrapEventMethodMetadata,
                                MethodDeclarationSyntax = methodNode,
                                CompilationUnitSyntax = compilationUnitSyntax,
                            });
                        var generate = codeFileGenerator.GenerateCode(codeFile);
                        var formatCode = CodeFormatter.FormatCode(generate);
                        yield return _scaffoldFileSystem.SaveFile(
                            $"{rootDirectoryName}/{claptrapMetadata.ClaptrapKind.Catalog}/Claptrap/N20EventMethods/{claptrapEventMethodMetadata.MethodInfo.Name}/{codeFile.FileName}",
                            formatCode);
                    }
                }
            }

            Task GE04()
            {
                var methodNodes = compilationUnitSyntax
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                    .ToArray();

                return Task.WhenAll(RunForEvent(claptrapMetadata.EventMethodMetadata));

                IEnumerable<Task> RunForEvent(IEnumerable<ClaptrapEventMethodMetadata> eventMethodMetadata)
                {
                    foreach (var claptrapEventMethodMetadata in eventMethodMetadata)
                    {
                        var methodNode = methodNodes.Single(x =>
                            x.Identifier.ToString() == claptrapEventMethodMetadata.MethodInfo.Name);
                        ICodeFileGenerator codeFileGenerator = new GE04CodeFileGenerator();
                        var codeFile = codeFileGenerator.CreateCodeFile(
                            new GE04CodeFileGeneratorContext
                            {
                                ClaptrapMetadata = claptrapMetadata,
                                ClaptrapEventMethodMetadata = claptrapEventMethodMetadata,
                                MethodDeclarationSyntax = methodNode,
                                CompilationUnitSyntax = compilationUnitSyntax,
                            });
                        var generate = codeFileGenerator.GenerateCode(codeFile);
                        var formatCode = CodeFormatter.FormatCode(generate);
                        yield return _scaffoldFileSystem.SaveFile(
                            $"{rootDirectoryName}/{claptrapMetadata.ClaptrapKind.Catalog}/Claptrap/N20EventMethods/{claptrapEventMethodMetadata.MethodInfo.Name}/{codeFile.FileName}",
                            formatCode);
                    }
                }
            }

            Task GE05()
            {
                return Task.WhenAll(RunForEvent(claptrapMetadata.ClaptrapEventMetadata));

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
                                StateDataType = claptrapMetadata.StateDataType,
                                CompilationUnitSyntax = compilationUnitSyntax,
                            });
                        var generate = codeFileGenerator.GenerateCode(codeFile);
                        var formatCode = CodeFormatter.FormatCode(generate);
                        yield return _scaffoldFileSystem.SaveFile(
                            $"{rootDirectoryName}/{claptrapMetadata.ClaptrapKind.Catalog}/Claptrap/N11StateDataUpdaters/{codeFile.FileName}",
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
                        StateDataType = claptrapMetadata.StateDataType,
                        CompilationUnitSyntax = compilationUnitSyntax,
                    });
                var generate = codeFileGenerator.GenerateCode(codeFile);
                var formatCode = CodeFormatter.FormatCode(generate);
                return _scaffoldFileSystem.SaveFile(
                    $"{rootDirectoryName}/{claptrapMetadata.ClaptrapKind.Catalog}/Claptrap/N10StateDataFactory/{codeFile.FileName}",
                    formatCode);
            }
        }
    }
}