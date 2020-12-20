using Autofac;
using Newbe.Claptrap.StorageTestWebApi.Services;

namespace Newbe.Claptrap.StorageTestWebApi
{
    public class StorageTestWebApiModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<TestService>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}