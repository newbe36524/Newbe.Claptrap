using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Design;

namespace Newbe.Claptrap.Bootstrapper
{
    internal static class LoggerFactoryHolder
    {
        /// <summary>
        /// Hold <see cref="ILoggerFactory"/> for creating <see cref="ILogger{TCategoryName}"/>.
        /// Since before while IClaptrapBootstrapperBuilder is building the bootstrapper, there is no way to inject logger.
        /// And some instance have to be activate with new keyword. e.g. <see cref="ClaptrapDesignStore"/> 
        /// </summary>
        public static ILoggerFactory Instance { get; internal set; } = null!;
    }
}