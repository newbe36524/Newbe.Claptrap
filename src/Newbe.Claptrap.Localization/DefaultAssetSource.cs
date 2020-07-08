using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lexical.Localization;
using Lexical.Localization.Asset;

namespace Newbe.Claptrap.Localization
{
    public class DefaultAssetSource : ILibraryAssetSources
    {
        private readonly List<IAssetSource> _list = new List<IAssetSource>();

        private readonly LineEmbeddedSource _defaultSource =
            LineReaderMap.Default.EmbeddedAssetSource(typeof(DefaultAssetSource).Assembly,
                typeof(DefaultAssetSource).Namespace + ".Docs.L.ini");


        public DefaultAssetSource()
        {
            // Add internal localization source
            _list.Add(_defaultSource);
            foreach (var lineEmbeddedSource in DefaultSources)
            {
                _list.Add(lineEmbeddedSource);
            }
        }

        public IEnumerator<IAssetSource> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static readonly IEnumerable<string> DefaultSupportedLanguages = new[]
        {
            "zh-Hans",
            "zh-Hant",
            "ja",
            "ru",
        };

        private static readonly IEnumerable<LineEmbeddedSource> DefaultSources = DefaultSupportedLanguages
            .Select(x => LineReaderMap.Default.EmbeddedAssetSource(typeof(DefaultAssetSource).Assembly,
                typeof(DefaultAssetSource).Namespace + $".Docs.L-{x}.ini"))
            .ToArray();
    }
}