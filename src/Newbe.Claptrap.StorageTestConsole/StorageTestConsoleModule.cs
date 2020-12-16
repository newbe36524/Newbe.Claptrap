using Autofac;
using Newbe.Claptrap.StorageTestConsole.Services;

namespace Newbe.Claptrap.StorageTestConsole
{
    public class StorageTestConsoleModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<EventInsertTestService>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}