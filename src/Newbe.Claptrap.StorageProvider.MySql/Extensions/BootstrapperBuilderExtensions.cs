using Newbe.Claptrap.StorageProvider.MySql;

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseMySqlAsStateStore(
            this IClaptrapBootstrapperBuilder builder)
            => builder.ConfigureGlobalClaptrapDesign(design =>
            {
                design.StateLoaderFactoryType = typeof(MySqlStateStoreFactory);
                design.StateSaverFactoryType = typeof(MySqlStateStoreFactory);
            });

        public static IClaptrapBootstrapperBuilder UseMySqlAsEventStore(
            this IClaptrapBootstrapperBuilder builder)
            => builder.ConfigureGlobalClaptrapDesign(design =>
            {
                design.EventLoaderFactoryType = typeof(MySqlEventStoreFactory);
                design.EventSaverFactoryType = typeof(MySqlEventStoreFactory);
            });
    }
}