using Newbe.Claptrap.MemoryStore;
using Newbe.Claptrap.StateHolder;

namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseMemoryAsStateStore(
            this IClaptrapBootstrapperBuilder builder)
            => builder
                .ConfigureClaptrapDesign(
                    x => x.StateLoaderFactoryType == null,
                    x =>
                        x.StateLoaderFactoryType = typeof(MemoryStateStoreFactory))
                .ConfigureClaptrapDesign(
                    x => x.StateSaverFactoryType == null,
                    x =>
                        x.StateSaverFactoryType = typeof(MemoryStateStoreFactory));

        public static IClaptrapBootstrapperBuilder UseMemoryAsEventStore(
            this IClaptrapBootstrapperBuilder builder)
            => builder
                .ConfigureClaptrapDesign(
                    x => x.EventLoaderFactoryType == null,
                    x =>
                        x.EventLoaderFactoryType = typeof(MemoryEventStoreFactory))
                .ConfigureClaptrapDesign(
                    x => x.EventSaverFactoryType == null,
                    x =>
                        x.EventSaverFactoryType = typeof(MemoryEventStoreFactory));


        public static IClaptrapBootstrapperBuilder UseNoChangeStateHolder(
            this IClaptrapBootstrapperBuilder builder)
            => builder.ConfigureClaptrapDesign(
                x => x.StateHolderFactoryType == null,
                x =>
                    x.StateHolderFactoryType = typeof(NoChangeStateHolderFactory));
    }
}