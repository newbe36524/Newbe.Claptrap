using System;
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

        public static IClaptrapBootstrapperBuilder UseSQLite(
            this IClaptrapBootstrapperBuilder builder,
            Action<ISQLiteProviderConfigurator> sqlite)
        {
            var sqLiteProviderConfigurator = new SQLiteProviderConfigurator(x => true, builder);
            sqlite(sqLiteProviderConfigurator);
            return builder;
        }

        public static IClaptrapBootstrapperBuilder UseSQLite(
            this IClaptrapBootstrapperBuilder builder,
            Func<IClaptrapDesign, bool> designFilter,
            Action<ISQLiteProviderConfigurator> sqlite)
        {
            var sqLiteProviderConfigurator = new SQLiteProviderConfigurator(designFilter, builder);
            sqlite(sqLiteProviderConfigurator);
            return builder;
        }
    }

    public interface ISQLiteProviderConfigurator
    {
        ISQLiteProviderConfigurator AsEventStore(Action<ISQLiteProviderEvenStoreConfigurator> eventStore);
        ISQLiteProviderConfigurator AsStateStore();
    }

    public interface ISQLiteProviderEvenStoreConfigurator
    {
        ISQLiteProviderEvenStoreConfigurator ConfigureOptions(Action<StorageProviderOptions> optionsAction);
    }

    public class SQLiteProviderEvenStoreConfigurator : ISQLiteProviderEvenStoreConfigurator
    {
        private readonly StorageProviderOptions _storageProviderOptions;

        public SQLiteProviderEvenStoreConfigurator(
            StorageProviderOptions storageProviderOptions)
        {
            _storageProviderOptions = storageProviderOptions;
        }

        public ISQLiteProviderEvenStoreConfigurator ConfigureOptions(
            Action<StorageProviderOptions> optionsAction)
        {
            optionsAction(_storageProviderOptions);
            return this;
        }
    }

    public static class SQLiteProviderEvenStoreConfiguratorExtensions
    {
        public static ISQLiteProviderEvenStoreConfigurator OneIdentityOneTable(
            this ISQLiteProviderEvenStoreConfigurator configurator)
        {
            var options = new SQLiteOneIdentityOneTableEventStoreOptions();
            configurator.ConfigureOptions(providerOptions =>
            {
                providerOptions.EventLoaderOptions = options;
                providerOptions.EventSaverOptions = options;
                // TODO move state options
                providerOptions.StateLoaderOptions = new EmptyStateLoaderOptions();
                providerOptions.StateSaverOptions = new EmptyStateSaverOptions();
            });
            return configurator;
        }
    }

    public class SQLiteProviderConfigurator : ISQLiteProviderConfigurator
    {
        private readonly Func<IClaptrapDesign, bool> _designFilter;
        private readonly IClaptrapBootstrapperBuilder _builder;

        public SQLiteProviderConfigurator(
            Func<IClaptrapDesign, bool> designFilter,
            IClaptrapBootstrapperBuilder builder)
        {
            _designFilter = designFilter;
            _builder = builder;
        }

        public ISQLiteProviderConfigurator AsEventStore(Action<ISQLiteProviderEvenStoreConfigurator> eventStore)
        {
            _builder
                .ConfigureClaptrapDesign(
                    _designFilter,
                    x =>
                    {
                        x.EventLoaderFactoryType = typeof(RelationalEventStoreFactory);
                        x.EventSaverFactoryType = typeof(RelationalEventStoreFactory);
                        var configurator = new SQLiteProviderEvenStoreConfigurator(x.StorageProviderOptions);
                        eventStore(configurator);
                    });
            return this;
        }

        public ISQLiteProviderConfigurator AsStateStore()
        {
            throw new System.NotImplementedException();
            return this;
        }
    }
}