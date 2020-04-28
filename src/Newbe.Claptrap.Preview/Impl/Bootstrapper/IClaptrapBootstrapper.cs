using Autofac;

namespace Newbe.Claptrap.Preview
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