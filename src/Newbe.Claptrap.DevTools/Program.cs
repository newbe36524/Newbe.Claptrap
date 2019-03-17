using System;
using Autofac;
using Newbe.Claptrap.Assemblies;
using Newbe.Claptrap.Autofac;
using Newbe.Claptrap.Autofac.Modules;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.DevTools
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<MetadataModule>();
            builder.Register(context =>
                    new ActorAssemblyProvider(new[] {typeof(IAccount).Assembly}))
                .As<IActorAssemblyProvider>();
            var container = builder.Build();

            var actorMetadataProvider = container.Resolve<IActorMetadataProvider>();
            var actorMetadataCollection = actorMetadataProvider.GetActorMetadata();
            foreach (var claptrapMetadata in actorMetadataCollection.ClaptrapMetadata)
            {
                Console.WriteLine(claptrapMetadata.InterfaceType.Name);
                foreach (var claptrapEventMetadata in claptrapMetadata.ClaptrapEventMetadata)
                {
                    Console.WriteLine(claptrapEventMetadata.EventType);
                }
            }

            foreach (var minionMetadata in actorMetadataCollection.MinionMetadata)
            {
                Console.WriteLine(minionMetadata.InterfaceType.Name);
            }

            Console.WriteLine(actorMetadataCollection);
        }
    }
}