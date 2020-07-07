using System;
using Newbe.Claptrap.Bootstrapper;

namespace Newbe.Claptrap.EventCenter.RabbitMQ.Extensions
{
    public class RabbitMQConfigurator
    {
        private readonly Func<IClaptrapDesign, bool> _designFilter;
        private readonly IClaptrapBootstrapperBuilder _builder;

        public RabbitMQConfigurator(
            Func<IClaptrapDesign, bool> designFilter,
            IClaptrapBootstrapperBuilder builder)
        {
            _designFilter = designFilter;
            _builder = builder;
        }

        public RabbitMQConfigurator AsEventCenter()
        {
            _builder.ConfigureClaptrapDesign(
                _designFilter,
                x => x.ClaptrapOptions.EventCenterOptions = new EventCenterOptions
                {
                    EventCenterType = EventCenterType.RabbitMQ
                });
            return this;
        }
    }
}