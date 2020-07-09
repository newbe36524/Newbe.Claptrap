using Autofac;
using Newbe.Claptrap.Design;
using Module = Autofac.Module;

namespace Newbe.Claptrap.Modules
{
    /// <summary>
    /// Module for building <see cref="IClaptrapDesignStoreFactory"/>
    /// </summary>
    public class ClaptrapDesignScanningModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ClaptrapDesignStoreValidator>()
                .As<IClaptrapDesignStoreValidator>()
                .SingleInstance();
            builder.RegisterType<ClaptrapDesignStoreFactory>()
                .As<IClaptrapDesignStoreFactory>()
                .SingleInstance();

            builder.RegisterType<AttributeBaseClaptrapDesignStoreProvider>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<ClaptrapDesignStoreCombiner>()
                .As<IClaptrapDesignStoreCombiner>()
                .SingleInstance();
            builder.RegisterType<ClaptrapDesignStore>()
                .AsSelf()
                .InstancePerDependency();
        }
    }
}