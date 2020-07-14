using System;

namespace Newbe.Claptrap.Bootstrapper
{
    public class OrleansConfigurator
    {
        private readonly Func<IClaptrapDesign, bool> _designFilter;
        private readonly IClaptrapBootstrapperBuilder _builder;

        public OrleansConfigurator(Func<IClaptrapDesign, bool> designFilter,
            IClaptrapBootstrapperBuilder builder)
        {
            _designFilter = designFilter;
            _builder = builder;
        }

        public OrleansConfigurator AsEventCenter()
        {
            _builder.ConfigureClaptrapDesign(_designFilter,
                x => { x.ClaptrapOptions.EventCenterOptions.EventCenterType = EventCenterType.OrleansClient; });
            return this;
        }
    }
}