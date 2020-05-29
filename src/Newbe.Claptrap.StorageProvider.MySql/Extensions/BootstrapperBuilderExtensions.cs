using System;
using Newbe.Claptrap.StorageProvider.MySql.Extensions;

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseMySql(
            this IClaptrapBootstrapperBuilder builder,
            Action<MySqlProviderConfigurator> mysql)
        {
            return builder.UseMySql(x => true, mysql);
        }

        public static IClaptrapBootstrapperBuilder UseMySql(
            this IClaptrapBootstrapperBuilder builder,
            Func<IClaptrapDesign, bool> designFilter,
            Action<MySqlProviderConfigurator> mysql)
        {
            var configurator = new MySqlProviderConfigurator(designFilter, builder);
            mysql(configurator);
            return builder;
        }
    }
}