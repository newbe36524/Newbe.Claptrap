using Autofac;
using Newbe.Claptrap.Autofac;
using Newbe.Claptrap.Demo.Impl.AccountImpl.Claptraps;
using Newbe.Claptrap.Demo.Impl.AccountImpl.Claptraps.EventMethods.AddBalanceImpl;
using Newbe.Claptrap.Demo.Impl.AccountImpl.Claptraps.EventMethods.LockImpl;
using Newbe.Claptrap.Demo.Impl.AccountImpl.Claptraps.EventMethods.TransferImpl;
using Newbe.Claptrap.Demo.Interfaces;

namespace Newbe.Claptrap.Demo
{
    public class DemoModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<Account>()
                .As<IAccount>();

            builder.RegisterType<AddBalanceMethod>()
                .As<IAddBalanceMethod>();
            builder.RegisterType<LockMethod>()
                .As<ILockMethod>();
            builder.RegisterType<TransferMethod>()
                .As<ITransferMethod>();

            var assemblies = new[] {typeof(IAccount).Assembly, typeof(DemoModule).Assembly};
            builder.RegisterDefaultStateDataFactories(assemblies);
            builder.RegisterUpdateStateDataHandlers(assemblies);
        }
    }
}