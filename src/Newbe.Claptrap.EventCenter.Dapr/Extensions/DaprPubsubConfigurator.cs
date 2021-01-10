using System;
using Newbe.Claptrap.Bootstrapper;

namespace Newbe.Claptrap.EventCenter.Dapr.Extensions
{
    public class DaprPubsubConfigurator
    {
        private readonly Func<IClaptrapDesign, bool> _designFilter;
        private readonly IClaptrapBootstrapperBuilder _builder;

        public DaprPubsubConfigurator(
            Func<IClaptrapDesign, bool> designFilter,
            IClaptrapBootstrapperBuilder builder)
        {
            _designFilter = designFilter;
            _builder = builder;
        }

        public DaprPubsubConfigurator AsEventCenter()
        {
            _builder.ConfigureClaptrapDesign(
                _designFilter,
                x => x.ClaptrapOptions.EventCenterOptions = new EventCenterOptions
                {
                    EventCenterType = EventCenterType.DaprPubsub
                });
            return this;
        }
    }
}