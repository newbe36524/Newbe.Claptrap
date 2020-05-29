using System;
using System.Collections.Generic;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder AddConnectionString(
            this IClaptrapBootstrapperBuilder builder,
            string dbName,
            string connectionString) =>
            builder.ConfigConnectionStrings(dictionary => dictionary[dbName] = connectionString);

        public static IClaptrapBootstrapperBuilder ConfigConnectionStrings(
            this IClaptrapBootstrapperBuilder builder,
            Action<IDictionary<string, string>> connectionStrings)
        {
            connectionStrings(builder.Options.StorageConnectionStrings);
            return builder;
        }
    }
}