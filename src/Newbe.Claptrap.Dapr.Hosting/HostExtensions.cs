using Dapr.Actors.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Newbe.Claptrap;
using Newbe.Claptrap.Hosting;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Hosting
{
    public static class HostExtensions
    {
        public static IHostBuilder UseClaptrapDaprHost(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((context, collection) =>
            {
                collection.Configure<ActorRuntimeOptions>(options =>
                {
                    var store = (IClaptrapDesignStore) context.Properties[ClaptrapHostConst.HostBuilderContextClaptrapDesignStoreKey];
                    foreach (var claptrapDesign in store)
                    {
                        var actorTypeInformation = ActorTypeInformation.Get(claptrapDesign.ClaptrapBoxImplementationType);
                        options.Actors.Add(
                            new ActorRegistration(
                                actorTypeInformation));
                    }
                });
            });
        }
    }
}