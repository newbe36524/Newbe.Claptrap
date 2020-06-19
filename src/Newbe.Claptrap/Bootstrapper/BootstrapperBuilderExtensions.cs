using Newbe.Claptrap.StateHolder;

namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseNoChangeStateHolder(
            this IClaptrapBootstrapperBuilder builder)
            => builder.ConfigureClaptrapDesign(
                x => x.StateHolderFactoryType == null,
                x =>
                    x.StateHolderFactoryType = typeof(NoChangeStateHolderFactory));
    }
}