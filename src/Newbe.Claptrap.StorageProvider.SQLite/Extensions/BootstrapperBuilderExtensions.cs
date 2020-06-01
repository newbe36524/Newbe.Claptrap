using System;
using Newbe.Claptrap.StorageProvider.SQLite.Extensions;

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseSQLite(
            this IClaptrapBootstrapperBuilder builder,
            Action<SQLiteProviderConfigurator> sqlite)
        {
            return builder.UseSQLite(x => true, sqlite);
        }

        public static IClaptrapBootstrapperBuilder UseSQLite(
            this IClaptrapBootstrapperBuilder builder,
            Func<IClaptrapDesign, bool> designFilter,
            Action<SQLiteProviderConfigurator> sqlite)
        {
            var sqLiteProviderConfigurator = new SQLiteProviderConfigurator(designFilter, builder);
            sqlite(sqLiteProviderConfigurator);
            return builder;
        }

        /// <summary>
        /// Use SQLite as storage in testing environment, it is almost ready for small unit test or integration test
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IClaptrapBootstrapperBuilder UseSQLiteAsTestingStorage(
            this IClaptrapBootstrapperBuilder builder) =>
            builder.UseSQLite(sqlite =>
                sqlite
                    .AsEventStore(evenStore => evenStore.SharedTable())
                    .AsStateStore(stateStore => stateStore.SharedTable()));
    }
}