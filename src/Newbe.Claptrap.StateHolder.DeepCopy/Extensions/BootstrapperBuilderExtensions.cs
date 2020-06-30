// ReSharper disable once CheckNamespace

using System;

namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseDeepClonerStateHolder(
            this IClaptrapBootstrapperBuilder builder)
            => builder.ConfigureClaptrapDesign(
                x => true,
                x =>
                    x.StateHolderFactoryType = typeof(DesignBaseEventHandlerFactory));

        public static IClaptrapBootstrapperBuilder UseDeepClonerStateHolder(
            this IClaptrapBootstrapperBuilder builder,
            Func<IClaptrapDesign, bool> predicate)
            => builder.ConfigureClaptrapDesign(
                predicate,
                x =>
                    x.StateHolderFactoryType = typeof(DesignBaseEventHandlerFactory));
    }
}