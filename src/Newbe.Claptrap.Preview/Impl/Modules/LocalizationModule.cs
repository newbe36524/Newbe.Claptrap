using Autofac;
using Lexical.Localization;
using Newbe.Claptrap.Preview.Impl.Localization;
using Module = Autofac.Module;

namespace Newbe.Claptrap.Preview.Impl.Modules
{
    public class LocalizationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            LK.Init();
            builder.Register(t => LocalizationRoot.Root.Type<L>().AsStringLocalizer())
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            builder.RegisterType<L>()
                .As<IL>()
                .SingleInstance();
            builder.RegisterBuildCallback(scope => { L.Instance = scope.Resolve<IL>(); });
        }
    }
}