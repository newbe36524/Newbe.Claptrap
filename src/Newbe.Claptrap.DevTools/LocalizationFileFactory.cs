using System;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

namespace Newbe.Claptrap.DevTools
{
    public class LocalizationFileFactory : ILocalizationFileFactory
    {
        public string Create(LocalizationFile file)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"# file is created by Newbe.Claptrap.DevTools at {DateTimeOffset.UtcNow:R}");
            if (string.IsNullOrEmpty(file.Culture))
            {
                sb.AppendLine("[Type:Newbe.Claptrap.L]");
            }
            else
            {
                sb.AppendLine($"[Culture:{file.Culture}:Type:Newbe.Claptrap.L]");
            }

            foreach (var item in file.Items)
            {
                sb.AppendLine($"# {item.SourceText}");
                sb.AppendLine($"{item.Key} = {item.Text}");
            }

            var re = sb.ToString();
            return re;
        }

        public LocalizationFile ResolveFormContent(string content)
        {
            var re = new LocalizationFile();
            if (content.StartsWith("[Type:Newbe.Claptrap.L]"))
            {
                re.Culture = string.Empty;
            }
            else
            {
                // [Culture:{file.Culture}:Type:Newbe.Claptrap.L] > 
                var culture = content.Substring(0, content.IndexOf("]", StringComparison.OrdinalIgnoreCase) + 1)
                    .Replace("[Culture:", string.Empty)
                    .Replace(":Type:Newbe.Claptrap.L]", string.Empty);
                re.Culture = culture;
            }

            var lines = content.Split(Environment.NewLine);
            var localizationItems = lines
                .ToObservable()
                .Skip(2)
                .Buffer(2)
                .Where(x => x.Count == 2 && x.All(a => !string.IsNullOrEmpty(a)))
                .Select(twoLine =>
                {
                    var sourceLine = twoLine[0];
                    var textLine = twoLine[1];
                    var x = textLine.Split("=");
                    var item = new LocalizationItem
                    {
                        Key = x[0].TrimEnd(),
                        Text = x[1].TrimStart(),
                        SourceText = sourceLine.TrimStart('#').TrimStart()
                    };
                    return item;
                })
                .ToEnumerable()
                .ToList();
            re.Items = localizationItems;
            return re;
        }
    }
}