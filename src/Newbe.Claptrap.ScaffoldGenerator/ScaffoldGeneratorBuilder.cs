using System.Reflection;
using Autofac;
using Newbe.Claptrap.Assemblies;
using Newbe.Claptrap.Autofac;
using Newbe.Claptrap.Autofac.Modules;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public class ScaffoldGeneratorBuilder : IScaffoldGeneratorBuilder
    {
        private class ScaffoldGeneratorBuilderContext
        {
            public Assembly InterfaceAssembly { get; set; }
            public string InterfaceProjectPath { get; set; }
            public string ScaffoldProjectPath { get; set; }
        }

        private readonly ScaffoldGeneratorBuilderContext _context = new ScaffoldGeneratorBuilderContext();

        public ScaffoldGeneratorBuilder SetInterfaceAssembly(Assembly assembly)
        {
            _context.InterfaceAssembly = assembly;
            return this;
        }

        public ScaffoldGeneratorBuilder SetInterfaceProjectPath(string path)
        {
            _context.InterfaceProjectPath = path;
            return this;
        }

        public ScaffoldGeneratorBuilder SetScaffoldProjectPath(string path)
        {
            _context.ScaffoldProjectPath = path;
            return this;
        }

        public IScaffoldGenerator Build()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<MetadataModule>();
            builder.Register(context =>
                    new ActorAssemblyProvider(new[] {_context.InterfaceAssembly}))
                .As<IActorAssemblyProvider>();
            builder.RegisterType<ScaffoldGenerator>()
                .As<IScaffoldGenerator>();

            builder.RegisterType<ClaptrapScaffoldGenerator>()
                .As<IClaptrapScaffoldGenerator>();
            builder.RegisterType<MinionScaffoldGenerator>()
                .As<IMinionScaffoldGenerator>();

            builder.Register(x =>
                    new ClaptrapInterfaceProjectFileProvider(
                        _context.InterfaceProjectPath))
                .As<IClaptrapInterfaceProjectFileProvider>()
                .SingleInstance();

            builder.Register(x => new ScaffoldFileSystem(_context.ScaffoldProjectPath))
                .As<IScaffoldFileSystem>()
                .SingleInstance();
            var container = builder.Build();

            var scaffoldGenerator = container.Resolve<IScaffoldGenerator>();
            var actorMetadataProvider = container.Resolve<IActorMetadataProvider>();
            scaffoldGenerator.Context = new ScaffoldGenerateContext
            {
                ActorMetadataCollection = actorMetadataProvider.GetActorMetadata(),
            };
            return scaffoldGenerator;
        }
    }
}