using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Newbe.Claptrap.Preview
{
    public class ClaptrapRegistrationFinder : IClaptrapRegistrationFinder
    {
        private readonly AttributeBaseActorTypeRegistrationProvider.Factory
            _attributeBaseActorTypeRegistrationProviderFactory;

        private readonly IActorTypeRegistrationCombiner _actorTypeRegistrationCombiner;

        public ClaptrapRegistrationFinder(
            AttributeBaseActorTypeRegistrationProvider.Factory attributeBaseActorTypeRegistrationProviderFactory,
            IActorTypeRegistrationCombiner actorTypeRegistrationCombiner)
        {
            _attributeBaseActorTypeRegistrationProviderFactory = attributeBaseActorTypeRegistrationProviderFactory;
            _actorTypeRegistrationCombiner = actorTypeRegistrationCombiner;
        }

        public ClaptrapRegistration Find(IEnumerable<Assembly> assemblies)
        {
            var actorTypeRegistrations = GetProviders().Select(x => x.ClaptrapRegistration);
            var re = _actorTypeRegistrationCombiner.Combine(actorTypeRegistrations);
            return re;

            IEnumerable<IActorTypeRegistrationProvider> GetProviders()
            {
                var attributeBaseActorTypeRegistrationProvider =
                    _attributeBaseActorTypeRegistrationProviderFactory.Invoke(assemblies);
                yield return attributeBaseActorTypeRegistrationProvider;
            }
        }
    }
}