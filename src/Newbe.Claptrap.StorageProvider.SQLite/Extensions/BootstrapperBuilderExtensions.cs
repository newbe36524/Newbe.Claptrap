using Newbe.Claptrap.StorageProvider.SQLite;

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseSQLiteAsStateStore(
            this IClaptrapBootstrapperBuilder builder)
            => builder.ConfigureGlobalClaptrapDesign(design =>
            {
                design.StateLoaderFactoryType = typeof(SQLiteStateStoreFactory);
                design.StateSaverFactoryType = typeof(SQLiteStateStoreFactory);
            });

        public static IClaptrapBootstrapperBuilder UseSQLiteAsEventStore(
            this IClaptrapBootstrapperBuilder builder)
            => builder.ConfigureGlobalClaptrapDesign(design =>
            {
                design.EventLoaderFactoryType = typeof(SQLiteEventStoreFactory);
                design.EventSaverFactoryType = typeof(SQLiteEventStoreFactory);
            });
    }
}