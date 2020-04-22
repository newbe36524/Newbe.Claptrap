using Autofac;

namespace Newbe.Claptrap.Autofac
{
    public interface IClaptrapBootstrapper
    {
        /// <summary>
        /// register all claptrap related services to the builder
        /// </summary>
        /// <param name="builder"></param>
        void RegisterServices(ContainerBuilder builder);
    }
}