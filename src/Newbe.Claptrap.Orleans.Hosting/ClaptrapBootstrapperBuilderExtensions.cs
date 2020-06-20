using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newbe.Claptrap.Orleans;
using Newbe.Claptrap.StorageProvider.Relational;

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap.Bootstrapper
{
    public static class ClaptrapBootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder AddDefaultConfiguration(
            this IClaptrapBootstrapperBuilder builder, HostBuilderContext context)
        {
            var config = context.Configuration.GetSection(ClaptrapServeringOptions.ConfigurationSectionName);
            var claptrapConfig = new ClaptrapServeringOptions();
            config.Bind(claptrapConfig);
            builder.AddConnectionString(Defaults.ConnectionName,
                claptrapConfig.DefaultConnectionString);
            return builder;
        }
    }
}