using Autofac;
using Newbe.Claptrap.StateHolder;

namespace Newbe.Claptrap.Modules
{
    public class NoChangeStateHolderModule : Module, IClaptrapAppModule
    {
        public string Name { get; } = "Claptrap NoChangeStateHolder module";

        public string Description { get; } =
            "Module for registering NoChangeStateHolder. NoChangeStateHolder will do nothing to state while copying state in event handler";

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<NoChangeStateHolder>()
                .AsSelf()
                .SingleInstance();
        }
    }
}