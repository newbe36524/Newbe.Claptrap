using Newbe.Claptrap.Design;

namespace Newbe.Claptrap.Bootstrapper
{
    public interface IClaptrapBootstrapper
    {
        /// <summary>
        /// register all claptrap related services to the builder
        /// </summary>
        void Boot();

        IClaptrapDesignStore DumpDesignStore();
    }
}