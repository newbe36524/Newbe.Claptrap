using System;
using Autofac;
using Newbe.Claptrap.DevTools.Translation;

namespace Newbe.Claptrap.DevTools
{
    public class ToolsServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<ToolService>()
                .As<IToolService>()
                .SingleInstance();
            builder.RegisterType<LocalizationFileFactory>()
                .As<ILocalizationFileFactory>()
                .SingleInstance();

            builder.RegisterType<Translator>()
                .AsSelf();
            builder.Register(t =>
                {
                    var factory = t.Resolve<Translator.Factory>();
                    return factory.Invoke("https://api.cognitive.microsofttranslator.com/",
                        "/translate?api-version=3.0",
                        Environment.GetEnvironmentVariable("AZURE_TRANSLATOR_SUBSCRIPTION_KEY"));
                })
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}