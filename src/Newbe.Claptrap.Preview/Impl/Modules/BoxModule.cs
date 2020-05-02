using Autofac;
using Newbe.Claptrap.Preview.Abstractions.Box;
using Newbe.Claptrap.Preview.Impl.Box;

namespace Newbe.Claptrap.Preview.Impl.Modules
{
    public class BoxModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ClaptrapBoxFactory>()
                .As<IClaptrapBoxFactory>()
                .InstancePerLifetimeScope();

            builder.RegisterType<NormalClaptrapBox>()
                .AsSelf()
                .InstancePerLifetimeScope();
        }
    }
}