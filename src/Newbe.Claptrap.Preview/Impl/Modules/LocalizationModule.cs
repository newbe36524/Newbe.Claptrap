using System.Globalization;
using Autofac;
using Lexical.Localization;
using Newbe.Claptrap.Preview.Impl.Localization;
using Module = Autofac.Module;

namespace Newbe.Claptrap.Preview.Impl.Modules
{
    public class LocalizationModule : Module
    {
        private readonly CultureInfo? _culture;

        public LocalizationModule(
            CultureInfo? culture = null)
        {
            _culture = culture;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            LK.Init();

            builder.Register(t =>
                {
                    var typeLine = (ILine) LocalizationRoot.Root.Type<L>();
                    if (_culture != null)
                    {
                        typeLine = typeLine.Culture(_culture);
                    }

                    return typeLine.AsStringLocalizer();
                })
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            builder.RegisterType<L>()
                .As<IL>()
                .SingleInstance();
            builder.RegisterBuildCallback(scope => { L.Instance = scope.Resolve<IL>(); });
        }
    }
}