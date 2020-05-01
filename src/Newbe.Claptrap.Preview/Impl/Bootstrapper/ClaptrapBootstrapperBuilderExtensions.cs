using System;
using Newbe.Claptrap.Preview.Abstractions.Metadata;
using Newbe.Claptrap.Preview.Impl.Metadata;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public static class ClaptrapBootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder AddOrReplaceDesign(this IClaptrapBootstrapperBuilder builder,
            params IClaptrapDesign[] claptrapDesigns)
        {
            builder.AddClaptrapDesignStoreProvider(new StaticClaptrapDesignStoreProvider(claptrapDesigns));
            return builder;
        }

        public static IClaptrapBootstrapperBuilder ConfigureClaptrapDesignStore(
            this IClaptrapBootstrapperBuilder builder,
            Action<IClaptrapDesignStore> action)
        {
            builder.AddClaptrapDesignStoreConfigurator(new FuncClaptrapDesignStoreConfigurator(action));
            return builder;
        }
    }
}