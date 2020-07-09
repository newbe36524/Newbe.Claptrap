using System.Globalization;
using Autofac;
using Lexical.Localization;
using Module = Autofac.Module;

namespace Newbe.Claptrap.Localization.Modules
{
    public class LocalizationModule : Module, IClaptrapApplicationModule
    {
        public string Name { get; } = "Localization module";
        public string Description { get; } = "Module for registering type for localization";
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