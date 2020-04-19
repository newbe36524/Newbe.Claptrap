using Autofac;
using Newbe.Claptrap.Autofac;

namespace Newbe.Claptrap.Demo
{
    public class ServerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<TransferAccountBalanceEventHandler>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterBuildCallback(scope =>
            {
                var register = scope.Resolve<IEventHandlerRegister>();
                register.RegisterHandler(typeof(AccountGrain).ToString(), "eventTy",
                    typeof(TransferAccountBalanceEventHandler));
            });

            builder.RegisterType<DefaultInitialStateDataFactoryHandler>()
                .Keyed<IInitialStateDataFactoryHandler>(typeof(AccountGrain).ToString())
                .InstancePerLifetimeScope();
            builder.RegisterBuildCallback(scope =>
            {
                var register = scope.Resolve<IStateDataTypeRegister>();
                register.RegisterStateDataType(typeof(AccountGrain).ToString(), typeof(AccountStateData));
            });
        }
    }
} 