// ReSharper disable once CheckNamespace

namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseDeepClonerStateHolder(
            this IClaptrapBootstrapperBuilder builder)
            => builder.ConfigureGlobalClaptrapDesign(design =>
            {
                design.StateHolderFactoryType = typeof(DesignBaseEventHandlerFactory);
            });
    }
}