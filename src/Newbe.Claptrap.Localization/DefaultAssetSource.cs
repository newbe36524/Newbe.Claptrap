using System.Collections;
using System.Collections.Generic;
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

        private readonly LineEmbeddedSource _cnSource =
            LineReaderMap.Default.EmbeddedAssetSource(typeof(DefaultAssetSource).Assembly,
                typeof(DefaultAssetSource).Namespace + ".Docs.L-cn.ini");

        public DefaultAssetSource()
        {
            // Add internal localization source
            _list.Add(_defaultSource);
            _list.Add(_cnSource);
        }

        public IEnumerator<IAssetSource> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}