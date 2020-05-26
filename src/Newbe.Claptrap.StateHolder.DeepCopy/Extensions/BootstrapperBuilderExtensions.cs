// ReSharper disable once CheckNamespace

namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseDeepClonerStateHolder(
            this IClaptrapBootstrapperBuilder builder)
            => builder.ConfigureClaptrapDesign(
                x => x.StateHolderFactoryType == null,
                x =>
                    x.StateHolderFactoryType = typeof(DesignBaseEventHandlerFactory));
    }
}