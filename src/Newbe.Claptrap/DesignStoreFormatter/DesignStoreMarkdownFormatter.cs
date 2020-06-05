using System.Linq;
using System.Text;
using Newbe.Claptrap.Extensions;

namespace Newbe.Claptrap.DesignStoreFormatter
{
    public class DesignStoreMarkdownFormatter : IClaptrapDesignStoreFormatter
    {
        private readonly DesignStoreMarkdownFormatterOptions _options;

        public DesignStoreMarkdownFormatter(DesignStoreMarkdownFormatterOptions options)
        {
            _options = options;
        }

        public string Format(IClaptrapDesignStore claptrapDesignStore)
        {
            var designs = claptrapDesignStore
                .OrderBy(x => x.MasterOfSelfTypeCode())
                .ThenBy(x => x.IsMinion())
                .ToArray();
            var sb = new StringBuilder();
            sb.AppendLine("# claptrap");
            foreach (var design in designs)
            {
                sb.AppendLine(OneDesign(design));
            }

            sb.AppendLine("# events");

            var eventDefs = designs.SelectMany(design => design.EventHandlerDesigns
                    .Select(eventHandlerDesign => new
                    {
                        design,
                        eventHandlerDesign,
                        masterClaptrapTypeCode = design.ClaptrapMasterDesign != null
                            ? design.ClaptrapMasterDesign.ClaptrapTypeCode
                            : design.ClaptrapTypeCode,
                        eventTypeCode = eventHandlerDesign.Key
                    }))
                .GroupBy(x => x.masterClaptrapTypeCode + x.eventTypeCode)
                .Select(x =>
                {
                    var first = x.First();
                    return new EventDef
                    {
                        EventTypeCode = first.eventHandlerDesign.Key,
                        MasterClaptrapTypeCode = first.masterClaptrapTypeCode,
                        EventHandlerDefs = x.Select(a => new EventHandlerDef
                            {
                                Handler = a.eventHandlerDesign.Value.EventHandlerType.Name,
                                ClaptrapTypeCode = a.design.ClaptrapTypeCode,
                            })
                            .ToArray(),
                    };
                });
            foreach (var eventDef in eventDefs)
            {
                sb.AppendLine(OneEvent(eventDef));
            }

            var re = sb.ToString();
            return re;
        }

        private string OneDesign(IClaptrapDesign claptrapDesign)
        {
            var sb = new StringBuilder();

            sb.AppendLine(
                claptrapDesign.IsMinion()
                    ? $"## {ClaptrapLink(claptrapDesign.ClaptrapTypeCode)} minion of {ClaptrapLink(claptrapDesign.ClaptrapMasterDesign.ClaptrapTypeCode)}"
                    : $"## {ClaptrapLink(claptrapDesign.ClaptrapTypeCode)}");


            sb.AppendLine("|event|handler|");
            sb.AppendLine("|---|---|");
            foreach (var (key, value) in claptrapDesign.EventHandlerDesigns)
            {
                sb.AppendLine(
                    $"|{EventLink(claptrapDesign.MasterOfSelfTypeCode(), key)}|{value.EventHandlerType.Name}|");
            }

            return sb.ToString();
        }

        private string OneEvent(EventDef eventDef)
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                $"## {EventLink(eventDef.MasterClaptrapTypeCode, eventDef.EventTypeCode)}");
            sb.AppendLine("|claptrap|handler|");
            sb.AppendLine("|---|---|");
            foreach (var eventHandlerDef in eventDef.EventHandlerDefs)
            {
                sb.AppendLine(
                    $"|{ClaptrapLink(eventHandlerDef.ClaptrapTypeCode)}|{eventHandlerDef.Handler}|");
            }

            return sb.ToString();
        }

        private class EventDef
        {
            public string MasterClaptrapTypeCode { get; set; } = null!;
            public string EventTypeCode { get; set; } = null!;
            public EventHandlerDef[] EventHandlerDefs { get; set; } = null!;
        }

        private class EventHandlerDef
        {
            public string ClaptrapTypeCode { get; set; } = null!;
            public string Handler { get; set; } = null!;
        }

        private string ClaptrapLink(string claptrapTypeCode)
        {
            var finalTypeCode = !string.IsNullOrEmpty(_options.TrimSuffix)
                ? claptrapTypeCode.Replace(_options.TrimSuffix, "")
                : claptrapTypeCode;

            return Link(finalTypeCode, FormatAnchor(finalTypeCode), claptrapTypeCode);
        }

        private string EventLink(string claptrapTypeCode, string eventTypeCode)
        {
            var finalClaptrapTypeCode = !string.IsNullOrEmpty(_options.ToString())
                ? claptrapTypeCode.Replace(_options.TrimSuffix, "")
                : claptrapTypeCode;

            var finalEventTypeCode = !string.IsNullOrEmpty(_options.ToString())
                ? eventTypeCode.Replace(_options.TrimSuffix, "")
                : eventTypeCode;

            var eventComposedKey = $"{finalClaptrapTypeCode}::{finalEventTypeCode}";
            return Link(eventComposedKey, FormatAnchor(eventComposedKey), eventTypeCode);
        }

        private static string Link(string text, string link, string title)
        {
            return $"[{text}]({link} \"{title}\")";
        }

        private static string FormatAnchor(string anchor)
        {
            return $"#{anchor.Replace("_", ".")}";
        }
    }
}