using System;
using System.Threading.Tasks;
using Autofac;
using Newbe.Claptrap.Assemblies;
using Newbe.Claptrap.Autofac;
using Newbe.Claptrap.Autofac.Modules;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Metadata;
using Newbe.Claptrap.ScaffoldGenerator;

namespace Newbe.Claptrap.DevTools
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<MetadataModule>();
            builder.Register(context =>
                    new ActorAssemblyProvider(new[] {typeof(IAccount).Assembly}))
                .As<IActorAssemblyProvider>();
            builder.RegisterType<ScaffoldGenerator.ScaffoldGenerator>()
                .As<IScaffoldGenerator>();
            var container = builder.Build();

            var scaffoldGenerator = container.Resolve<IScaffoldGenerator>();
            var actorMetadataProvider = container.Resolve<IActorMetadataProvider>();
            await scaffoldGenerator.Generate(new ScaffoldGenerateContext
            {
                ActorMetadataCollection = actorMetadataProvider.GetActorMetadata(),
            });
        }
    }
}