using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newbe.Claptrap.DevTools.Translation;

namespace Newbe.Claptrap.DevTools
{
    public class ToolService : IToolService
    {
        private readonly ILocalizationFileFactory _localizationFileFactory;
        private readonly ITranslator _translator;

        public ToolService(
            ILocalizationFileFactory localizationFileFactory,
            ITranslator translator)
        {
            _localizationFileFactory = localizationFileFactory;
            _translator = translator;
        }

        public async Task RunAsync()
        {
            var fileContent =
                await File.ReadAllTextAsync(
                    "../../../../Newbe.Claptrap.Localization/LK.cs");
            var root = CSharpSyntaxTree.ParseText(fileContent).GetCompilationUnitRoot();
            var items = GetLocalizationItems().OrderBy(x => x.Key).ToArray();
            var localizationFile = new LocalizationFile
            {
                Culture = string.Empty,
                Items = items
            };
            var globalFile = _localizationFileFactory.Create(localizationFile);

            var translationResults = await _translator.TranslateAsync(localizationFile, new[]
            {
                "zh-Hans",
                "zh-Hant",
                "ja",
                "ru",
            });

            const string globalFilePath = "../../../../Newbe.Claptrap.Localization/Docs/L.ini";
            await File.WriteAllTextAsync(globalFilePath, globalFile);

            foreach (var (k,v) in translationResults)
            {
                var cnFilePath = $"../../../../Newbe.Claptrap.Localization/Docs/L-{k}.ini";
                var createNew = true;
                if (createNew)
                {
                    var cnFile = _localizationFileFactory.Create(v);
                    await File.WriteAllTextAsync(cnFilePath, cnFile);
                }
                else
                {
                    var oldFileString = await File.ReadAllTextAsync(cnFilePath);
                    var oldFile = _localizationFileFactory.ResolveFormContent(oldFileString);
                    var oldFileItemDic = oldFile.Items.ToDictionary(x => x.Key);

                    var cnItems = CreateNewItems().OrderBy(x => x.Key).ToArray();
                    var cnFile = _localizationFileFactory.Create(new LocalizationFile
                    {
                        Culture = "cn",
                        Items = cnItems
                    });
                    await File.WriteAllTextAsync(cnFilePath, cnFile);

                    IEnumerable<LocalizationItem> CreateNewItems()
                    {
                        foreach (var item in v.Items!)
                        {
                            if (oldFileItemDic!.TryGetValue(item.Key, out var oldItem))
                            {
                                if (oldItem.SourceText != item.SourceText)
                                {
                                    yield return item;
                                }
                                else
                                {
                                    yield return oldItem;
                                }
                            }
                            else
                            {
                                yield return item;
                            }
                        }
                    }
                }
            }

            IEnumerable<LocalizationItem> GetLocalizationItems()
            {
                Debug.Assert(root != null, nameof(root) + " != null");
                var properties = root.DescendantNodes()
                    .OfType<PropertyDeclarationSyntax>()
                    .ToArray();
                foreach (var propertyDeclarationSyntax in properties)
                {
                    // L####XXX
                    var className = ((ClassDeclarationSyntax) propertyDeclarationSyntax.Parent)!.Identifier.ToString();
                    // L###XXX
                    var propertyName = propertyDeclarationSyntax.Identifier.ToString();
                    var text = propertyDeclarationSyntax.GetLeadingTrivia()
                        .Single(x => x.Kind() == SyntaxKind.SingleLineDocumentationCommentTrivia).ToString()
                        .Replace("<summary>", string.Empty)
                        .Replace("</summary>", string.Empty)
                        .Replace("///", string.Empty)
                        .Trim();
                    var item = new LocalizationItem
                    {
                        Text = text,
                        Key =
                            $"Key:LK.{className.Substring(0, "L0000".Length)}.{propertyName.Substring(1, "XXX".Length)}",
                        SourceText = text,
                    };
                    yield return item;
                }
            }
        }
    }
}