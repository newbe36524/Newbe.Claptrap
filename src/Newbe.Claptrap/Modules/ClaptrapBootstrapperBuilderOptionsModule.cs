using Autofac;

namespace Newbe.Claptrap.Modules
{
    public class ClaptrapBootstrapperBuilderOptionsModule : Module
    {
        private readonly ClaptrapBootstrapperBuilderOptions _claptrapBootstrapperBuilderOptions;

        public ClaptrapBootstrapperBuilderOptionsModule(
            ClaptrapBootstrapperBuilderOptions claptrapBootstrapperBuilderOptions)
        {
            _claptrapBootstrapperBuilderOptions = claptrapBootstrapperBuilderOptions;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterInstance(_claptrapBootstrapperBuilderOptions)
                .AsSelf()
                .SingleInstance();
        }
    }
}