using System;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Extensions;

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UsePostgreSQL(
            this IClaptrapBootstrapperBuilder builder,
            Action<PostgreSQLProviderConfigurator> postgreSQL)
        {
            return builder.UsePostgreSQL(x => true, postgreSQL);
        }

        public static IClaptrapBootstrapperBuilder UsePostgreSQL(
            this IClaptrapBootstrapperBuilder builder,
            Func<IClaptrapDesign, bool> designFilter,
            Action<PostgreSQLProviderConfigurator> postgreSQL)
        {
            var configurator = new PostgreSQLProviderConfigurator(designFilter, builder);
            postgreSQL(configurator);
            return builder;
        }
    }
}