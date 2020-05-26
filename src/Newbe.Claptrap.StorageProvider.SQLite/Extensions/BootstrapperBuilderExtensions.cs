using Newbe.Claptrap.StorageProvider.SQLite;

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseSQLiteAsStateStore(
            this IClaptrapBootstrapperBuilder builder)
            => builder
                .ConfigureClaptrapDesign(
                    x => x.StateLoaderFactoryType == null,
                    x =>
                        x.StateLoaderFactoryType = typeof(SQLiteStateStoreFactory))
                .ConfigureClaptrapDesign(
                    x => x.StateSaverFactoryType == null,
                    x =>
                        x.StateSaverFactoryType = typeof(SQLiteStateStoreFactory));

        public static IClaptrapBootstrapperBuilder UseSQLiteAsEventStore(
            this IClaptrapBootstrapperBuilder builder)
            => builder.ConfigureClaptrapDesign(
                    x => x.EventLoaderFactoryType == null,
                    x =>
                        x.EventLoaderFactoryType = typeof(SQLiteEventStoreFactory))
                .ConfigureClaptrapDesign(
                    x => x.EventSaverFactoryType == null,
                    x =>
                        x.EventSaverFactoryType = typeof(SQLiteEventStoreFactory));
    }
}