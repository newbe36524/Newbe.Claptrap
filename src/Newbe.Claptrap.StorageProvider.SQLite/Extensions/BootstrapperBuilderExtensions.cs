using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.SQLite;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

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
            => builder
                .ConfigureClaptrapDesign(
                    x =>
                    {
                        x.EventLoaderFactoryType = typeof(RelationalEventStoreFactory);
                        x.EventSaverFactoryType = typeof(RelationalEventStoreFactory);
                        var options = new SQLiteOneIdentityOneTableEventStoreOptions();
                        x.StorageProviderOptions.EventLoaderOptions = options;
                        x.StorageProviderOptions.EventSaverOptions = options;
                    });
    }
}