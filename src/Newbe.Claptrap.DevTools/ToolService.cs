using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Newbe.Claptrap.DevTools
{
    public class ToolService : IToolService
    {
        private readonly ILocalizationFileFactory _localizationFileFactory;

        public ToolService(
            ILocalizationFileFactory localizationFileFactory)
        {
            _localizationFileFactory = localizationFileFactory;
        }

        public async Task RunAsync()
        {
            var fileContent =
                await File.ReadAllTextAsync(
                    "../../../../Newbe.Claptrap.Preview/Impl/Localization/LK.cs");
            var root = CSharpSyntaxTree.ParseText(fileContent).GetCompilationUnitRoot();
            var items = GetLocalizationItems().OrderBy(x => x.Key).ToArray();
            var globalFile = _localizationFileFactory.Create(new LocalizationFile
            {
                Culture = string.Empty,
                Items = items
            });

            const string globalFilePath = "../../../../Newbe.Claptrap.Preview/Docs/L.ini";
            await File.WriteAllTextAsync(globalFilePath, globalFile);

            const string cnFilePath = "../../../../Newbe.Claptrap.Preview/Docs/L-cn.ini";
            var createNew = false;
            if (createNew)
            {
                var cnFile = _localizationFileFactory.Create(new LocalizationFile
                {
                    Culture = "cn",
                    Items = items
                });
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
                    foreach (var item in items!)
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