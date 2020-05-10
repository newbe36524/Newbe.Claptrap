using System.Collections.Generic;
using Lexical.Localization;
using Lexical.Localization.Asset;

namespace Newbe.Claptrap.Localization
{
    public class DefaultAssetSource : List<IAssetSource>, ILibraryAssetSources
    {
        private readonly LineEmbeddedSource _defaultSource =
            LineReaderMap.Default.EmbeddedAssetSource(typeof(DefaultAssetSource).Assembly,
                typeof(DefaultAssetSource).Namespace + ".Docs.L.ini");

        private readonly LineEmbeddedSource _cnSource =
            LineReaderMap.Default.EmbeddedAssetSource(typeof(DefaultAssetSource).Assembly,
                typeof(DefaultAssetSource).Namespace + ".Docs.L-cn.ini");

        public DefaultAssetSource() : base()
        {
            // Add internal localization source
            Add(_defaultSource);
            Add(_cnSource);
        }
    }
}