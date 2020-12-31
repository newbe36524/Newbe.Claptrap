using System;

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseDeepClonerStateHolder(
            this IClaptrapBootstrapperBuilder builder)
        {
            return builder.ConfigureClaptrapDesign(
                x => true,
                x =>
                    x.StateHolderFactoryType = typeof(DesignBaseEventHandlerFactory));
        }

        public static IClaptrapBootstrapperBuilder UseDeepClonerStateHolder(
            this IClaptrapBootstrapperBuilder builder,
            Func<IClaptrapDesign, bool> predicate)
        {
            return builder.ConfigureClaptrapDesign(
                predicate,
                x =>
                    x.StateHolderFactoryType = typeof(DesignBaseEventHandlerFactory));
        }
    }
}