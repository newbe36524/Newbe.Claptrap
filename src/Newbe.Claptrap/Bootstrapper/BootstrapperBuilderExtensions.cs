using Newbe.Claptrap.MemoryStore;

namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseMemoryAsStateStore(
            this IClaptrapBootstrapperBuilder builder)
            => builder.ConfigureGlobalClaptrapDesign(design =>
            {
                design.StateLoaderFactoryType = typeof(MemoryStateStoreFactory);
                design.StateSaverFactoryType = typeof(MemoryStateStoreFactory);
            });

        public static IClaptrapBootstrapperBuilder UseMemoryAsEventStore(
            this IClaptrapBootstrapperBuilder builder)
            => builder.ConfigureGlobalClaptrapDesign(design =>
            {
                design.EventLoaderFactoryType = typeof(MemoryEventStoreFactory);
                design.EventSaverFactoryType = typeof(MemoryEventStoreFactory);
            });

        public static IClaptrapBootstrapperBuilder UseNoChangeStateHolder(
            this IClaptrapBootstrapperBuilder builder)
            => builder.ConfigureGlobalClaptrapDesign(design =>
            {
                design.StateHolderFactoryType = typeof(NoChangeStateHolderFactory);
            });
    }
}