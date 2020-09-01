using System;

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseOrleans(
            this IClaptrapBootstrapperBuilder builder,
            Action<OrleansConfigurator> orleans)
        {
            return builder.UseOrleans(x => true, orleans);
        }

        public static IClaptrapBootstrapperBuilder UseOrleans(
            this IClaptrapBootstrapperBuilder builder,
            Func<IClaptrapDesign, bool> designFilter,
            Action<OrleansConfigurator> orleans)
        {
            var configurator = new OrleansConfigurator(designFilter, builder);
            orleans(configurator);
            return builder;
        }
    }
}