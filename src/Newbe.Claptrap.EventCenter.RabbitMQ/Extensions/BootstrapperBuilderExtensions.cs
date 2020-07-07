using System;
using Newbe.Claptrap.EventCenter.RabbitMQ.Extensions;
// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap.Bootstrapper
{
    public static class BootstrapperBuilderExtensions
    {
        public static IClaptrapBootstrapperBuilder UseRabbitMQ(
            this IClaptrapBootstrapperBuilder builder,
            Action<RabbitMQConfigurator> rabbitmq)
            => builder.UseRabbitMQ(x => true, rabbitmq);

        public static IClaptrapBootstrapperBuilder UseRabbitMQ(
            this IClaptrapBootstrapperBuilder builder,
            Func<IClaptrapDesign, bool> designFilter,
            Action<RabbitMQConfigurator> rabbitmq)
        {
            var rabbitMQConfigurator = new RabbitMQConfigurator(designFilter, builder);
            rabbitmq(rabbitMQConfigurator);
            return builder;
        }
    }
}