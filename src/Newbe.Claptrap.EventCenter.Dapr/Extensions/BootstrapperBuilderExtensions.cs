using System;
using Newbe.Claptrap.EventCenter.Dapr.Extensions;

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseDaprPubsub(
            this IClaptrapBootstrapperBuilder builder,
            Action<DaprPubsubConfigurator> rabbitmq)
        {
            return builder.UseDaprPubsub(x => true, rabbitmq);
        }

        public static IClaptrapBootstrapperBuilder UseDaprPubsub(
            this IClaptrapBootstrapperBuilder builder,
            Func<IClaptrapDesign, bool> designFilter,
            Action<DaprPubsubConfigurator> rabbitmq)
        {
            var configurator = new DaprPubsubConfigurator(designFilter, builder);
            rabbitmq(configurator);
            return builder;
        }
    }
}