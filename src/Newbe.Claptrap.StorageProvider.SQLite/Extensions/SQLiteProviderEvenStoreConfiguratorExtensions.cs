using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.Bootstrapper
{
    public static class SQLiteProviderEvenStoreConfiguratorExtensions
    {
        public static ISQLiteProviderEvenStoreConfigurator OneIdentityOneTable(
            this ISQLiteProviderEvenStoreConfigurator configurator)
        {
            configurator.ConfigureOptions(providerOptions =>
            {
                var eventOptions = new SQLiteOneIdentityOneTableEventStoreOptions();
                providerOptions.EventLoaderOptions = eventOptions;
                providerOptions.EventSaverOptions = eventOptions;
                var stateOptions = new SQLiteOneIdentityOneTableStateStoreOptions();
                providerOptions.StateLoaderOptions = stateOptions;
                providerOptions.StateSaverOptions = stateOptions;
            });
            return configurator;
        }
    }
}