using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap.Bootstrapper
{
    public static class ClaptrapBootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder AddConfiguration(
            this IClaptrapBootstrapperBuilder builder,
            IConfiguration configuration)
        {
            var config = configuration.GetSection(ClaptrapServerOptions.ConfigurationSectionName);
            var claptrapConfig = new ClaptrapServerOptions();

            config.Bind(claptrapConfig);
            builder.AddConnectionString(Defaults.ConnectionName,
                claptrapConfig.DefaultConnectionString);
            foreach (var (key, value) in claptrapConfig.ConnectionStrings)
            {
                builder.AddConnectionString(key, value);
            }

            LoadEventStore(builder, claptrapConfig.EventStore);
            LoadStateStore(builder, claptrapConfig.StateStore);
            return builder;
        }

        private static void LoadStateStore(IClaptrapBootstrapperBuilder builder, StorageOptions options)
        {
            switch (options.DatabaseType)
            {
                case DatabaseType.SQLite:
                case DatabaseType.PostgreSQL:
                case DatabaseType.MySql:
                case DatabaseType.MongoDB:
                    Assembly.Load($"Newbe.Claptrap.StorageProvider.{options.DatabaseType:G}");
                    InvokeFactory();
                    break;
                case DatabaseType.Known:
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(options), "unsupported database type");
            }

            void InvokeFactory()
            {
                var methodName = $"Use{options.DatabaseType}AsStateStore";
                var method = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => x.FullName.StartsWith("Newbe.Claptrap"))
                    .SelectMany(x => x.GetTypes())
                    .Where(x => x.Namespace == "Newbe.Claptrap.Bootstrapper" &&
                                x.Name.Contains("BootstrapperBuilderExtensions") && x.GetMethod(methodName) != null)
                    .Select(x => x.GetMethod(methodName))
                    .FirstOrDefault();
                Debug.Assert(method != null,
                    "method != null failed",
                    "failed to find method {0}, please add the specific data storage assembly",
                    methodName);
                method.Invoke(null, new object[] {builder, options});
            }
        }

        private static void LoadEventStore(IClaptrapBootstrapperBuilder builder, StorageOptions options)
        {
            switch (options.DatabaseType)
            {
                case DatabaseType.SQLite:
                case DatabaseType.PostgreSQL:
                case DatabaseType.MySql:
                case DatabaseType.MongoDB:
                    Assembly.Load($"Newbe.Claptrap.StorageProvider.{options.DatabaseType:G}");
                    InvokeFactory();
                    break;
                case DatabaseType.Known:
                    return;
                default:
                    throw new ArgumentOutOfRangeException(nameof(options), "unsupported database type");
            }

            void InvokeFactory()
            {
                var methodName = $"Use{options.DatabaseType}AsEventStore";
                var method = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => x.FullName.StartsWith("Newbe.Claptrap"))
                    .SelectMany(x => x.GetTypes())
                    .Where(x => x.Namespace == "Newbe.Claptrap.Bootstrapper" &&
                                x.Name.Contains("BootstrapperBuilderExtensions") && x.GetMethod(methodName) != null)
                    .Select(x => x.GetMethod(methodName))
                    .FirstOrDefault();
                Debug.Assert(method != null,
                    "method != null failed",
                    "failed to find method {0}, please add the specific data storage assembly",
                    methodName);
                method.Invoke(null, new object[] {builder, options});
            }
        }
    }
}