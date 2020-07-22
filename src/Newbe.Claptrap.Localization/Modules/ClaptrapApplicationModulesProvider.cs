using System.Collections.Generic;

namespace Newbe.Claptrap.Localization.Modules
{
    public class ClaptrapApplicationModulesProvider : IClaptrapApplicationModulesProvider
    {
        private readonly ClaptrapBootstrapperBuilderOptions _claptrapBootstrapperBuilderOptions;

        public ClaptrapApplicationModulesProvider(
            ClaptrapBootstrapperBuilderOptions claptrapBootstrapperBuilderOptions)
        {
            _claptrapBootstrapperBuilderOptions = claptrapBootstrapperBuilderOptions;
        }

        public IEnumerable<IClaptrapApplicationModule> GetClaptrapApplicationModules()
        {
            yield return new LocalizationModule(_claptrapBootstrapperBuilderOptions.ClaptrapLocalizationOptions);
        }
    }
}