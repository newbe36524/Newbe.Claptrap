using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Newbe.Claptrap.Design;

namespace Newbe.Claptrap.Bootstrapper
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

        public static IClaptrapBootstrapperBuilder ConfigureClaptrapDesign(
            this IClaptrapBootstrapperBuilder builder,
            Action<IClaptrapDesign> action)
        {
            builder.Options.ClaptrapDesignStoreConfigurators.Add(new FuncClaptrapDesignStoreConfigurator(store =>
            {
                foreach (var claptrapDesign in store)
                {
                    action(claptrapDesign);
                }
            }));
            return builder;
        }

        public static IClaptrapBootstrapperBuilder ConfigureClaptrapDesign(
            this IClaptrapBootstrapperBuilder builder,
            Func<IClaptrapDesign, bool> predicate,
            Action<IClaptrapDesign> action)
        {
            builder.Options.ClaptrapDesignStoreConfigurators.Add(new FuncClaptrapDesignStoreConfigurator(store =>
            {
                foreach (var claptrapDesign in store.Where(predicate))
                {
                    action(claptrapDesign);
                }
            }));
            return builder;
        }

        /// <summary>
        /// Add assemblies for scanning claptrap design
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static IClaptrapBootstrapperBuilder ScanClaptrapDesigns(
            this IClaptrapBootstrapperBuilder builder,
            IEnumerable<Type> types)
        {
            builder.Options.DesignTypes = builder.Options.DesignTypes.Concat(types);
            return builder;
        }

        /// <summary>
        /// Add assemblies for scanning claptrap design
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IClaptrapBootstrapperBuilder ScanClaptrapDesigns(
            this IClaptrapBootstrapperBuilder builder,
            IEnumerable<Assembly> assemblies)
        {
            return builder.ScanClaptrapDesigns(assemblies.SelectMany(x => x.GetTypes()));
        }

        /// <summary>
        /// Add assemblies for scanning claptrap module
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IClaptrapBootstrapperBuilder ScanClaptrapModule(
            this IClaptrapBootstrapperBuilder builder,
            IEnumerable<Assembly> assemblies)
        {
            return builder.ScanClaptrapModule(assemblies.SelectMany(x => x.GetTypes()));
        }

        /// <summary>
        /// Add types for scanning claptrap modules
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public static IClaptrapBootstrapperBuilder ScanClaptrapModule(
            this IClaptrapBootstrapperBuilder builder,
            IEnumerable<Type> types)
        {
            builder.Options.ModuleTypes = builder.Options.ModuleTypes.Concat(types);
            return builder;
        }

        /// <summary>
        /// Add all assemblies related to Claptrap in application bin directory as claptrap module assembly
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IClaptrapBootstrapperBuilder ScanClaptrapModule(
            this IClaptrapBootstrapperBuilder builder)
        {
            AssemblyHelper.ScanAndLoadClaptrapAssemblies(AppDomain.CurrentDomain.BaseDirectory,
                filePath => filePath.Contains("Claptrap"));
            var claptrapAssemblies =
                AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("Claptrap"));
            builder.ScanClaptrapModule(claptrapAssemblies);
            return builder;
        }

        /// <summary>
        /// Set current culture info about claptrap system.
        /// It will change language about all exception and message about claptrap system.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public static IClaptrapBootstrapperBuilder SetCultureInfo(
            this IClaptrapBootstrapperBuilder builder,
            CultureInfo cultureInfo)
        {
            return builder;
        }

        /// <summary>
        /// Configure builder options. 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configAction"></param>
        /// <returns></returns>
        public static IClaptrapBootstrapperBuilder ConfigureOptions(
            this IClaptrapBootstrapperBuilder builder,
            Action<ClaptrapBootstrapperBuilderOptions> configAction)
        {
            configAction(builder.Options);
            return builder;
        }

        /// <summary>
        /// Set design validation
        /// </summary>
        /// <returns></returns>
        public static IClaptrapBootstrapperBuilder SetDesignValidation(
            this IClaptrapBootstrapperBuilder builder,
            bool enabled)
        {
            builder.ConfigureOptions(option => { option.ClaptrapDesignStoreValidationOptions.Enabled = enabled; });
            return builder;
        }
    }
}