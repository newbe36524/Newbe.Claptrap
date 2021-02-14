using Dapr.Actors.Runtime;
using Newbe.Claptrap;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting
{
    public static class HostExtensions
    {
        public static ActorRuntimeOptions AddClaptrapDesign(this ActorRuntimeOptions options,
            IClaptrapDesignStore store)
        {
            foreach (var claptrapDesign in store)
            {
                var actorTypeInformation =
                    ActorTypeInformation.Get(claptrapDesign.ClaptrapBoxImplementationType);
                options.Actors.Add(
                    new ActorRegistration(
                        actorTypeInformation));
            }

            return options;
        }
    }
}