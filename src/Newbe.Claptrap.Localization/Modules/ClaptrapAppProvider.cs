using System.Collections.Generic;

namespace Newbe.Claptrap.Localization.Modules
{
    public class ClaptrapAppProvider : IClaptrapAppProvider
    {
        private readonly ClaptrapBootstrapperBuilderOptions _claptrapBootstrapperBuilderOptions;

        public ClaptrapAppProvider(
            ClaptrapBootstrapperBuilderOptions claptrapBootstrapperBuilderOptions)
        {
            _claptrapBootstrapperBuilderOptions = claptrapBootstrapperBuilderOptions;
        }

        public IEnumerable<IClaptrapAppModule> GetClaptrapApplicationModules()
        {
            yield return new LocalizationModule(_claptrapBootstrapperBuilderOptions.ClaptrapLocalizationOptions);
        }
    }
}