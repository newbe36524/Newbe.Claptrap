using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Newbe.Claptrap.Preview.Abstractions.Design;
using Newbe.Claptrap.Preview.Impl.Design;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public static class ClaptrapBootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder AddOrReplaceDesign(this IClaptrapBootstrapperBuilder builder,
            params IClaptrapDesign[] claptrapDesigns)
        {
            builder.Options.ClaptrapDesignStoreProviders.Add(new StaticClaptrapDesignStoreProvider(claptrapDesigns));
            return builder;
        }

        public static IClaptrapBootstrapperBuilder ConfigureClaptrapDesignStore(
            this IClaptrapBootstrapperBuilder builder,
            Action<IClaptrapDesignStore> action)
        {
            builder.Options.ClaptrapDesignStoreConfigurators.Add(new FuncClaptrapDesignStoreConfigurator(action));
            return builder;
        }

        public static IClaptrapBootstrapperBuilder ConfigureGlobalClaptrapDesign(
            this IClaptrapBootstrapperBuilder builder,
            Action<IGlobalClaptrapDesign> action)
        {
            var design = new GlobalClaptrapDesign();
            action.Invoke(design);
            builder.Options.ClaptrapDesignStoreConfigurators.Add(new GlobalClaptrapDesignStoreConfigurator(design));
            return builder;
        }

        public static IClaptrapBootstrapperBuilder AddAssemblies(
            this IClaptrapBootstrapperBuilder builder,
            IEnumerable<Assembly> assemblies)
        {
            builder.Options.ScanningAssemblies = builder.Options.ScanningAssemblies.Concat(assemblies);
            return builder;
        }

        public static IClaptrapBootstrapperBuilder SetCultureInfo(
            this IClaptrapBootstrapperBuilder builder,
            CultureInfo cultureInfo)
        {
            builder.Options.CultureInfo = cultureInfo;
            return builder;
        }

        public static IClaptrapBootstrapperBuilder ConfigureOptions(
            this IClaptrapBootstrapperBuilder builder,
            Action<ClaptrapBootstrapperBuilderOptions> configAction)
        {
            configAction(builder.Options);
            return builder;
        }
    }
}