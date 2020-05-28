using System;

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseSQLite(
            this IClaptrapBootstrapperBuilder builder,
            Action<SQLiteProviderConfigurator> sqlite)
        {
            var sqLiteProviderConfigurator = new SQLiteProviderConfigurator(x => true, builder);
            sqlite(sqLiteProviderConfigurator);
            return builder;
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
    }
}