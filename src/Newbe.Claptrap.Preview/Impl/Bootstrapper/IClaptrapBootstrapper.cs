using Autofac;
using Newbe.Claptrap.Preview.Abstractions.Metadata;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public interface IClaptrapBootstrapper
    {
        /// <summary>
        /// register all claptrap related services to the builder
        /// </summary>
        /// <param name="builder"></param>
        void RegisterServices(ContainerBuilder builder);

        IClaptrapDesignStore DumpDesignStore();
    }
}