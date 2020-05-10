using Lexical.Localization;
using Lexical.Localization.Asset;

namespace Newbe.Claptrap.Localization
{
    internal class LocalizationRoot : LineRoot.LinkedTo
    {
        /// <summary>
        /// Singleton instance to localization root for this class library.
        /// </summary>
        public static LocalizationRoot Root { get; } = new LocalizationRoot(Global);

        /// <summary>
        /// Add asset sources here. Then call <see cref="IAssetBuilder.Build"/> to make effective.
        /// </summary>
        private new static IAssetBuilder Builder => LineRoot.Builder;

        private LocalizationRoot(ILine linkedTo) : base(null, linkedTo, null, null, null, null, null)
        {
            // Add library's internal assets here
            Builder.AddSources(new DefaultAssetSource());
            // Apply changes
            Builder.Build();
        }
    }
}