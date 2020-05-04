using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Design;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public class GlobalClaptrapDesignStoreConfigurator : IClaptrapDesignStoreConfigurator
    {
        private readonly IGlobalClaptrapDesign _globalClaptrapDesign;
        private readonly ILogger<GlobalClaptrapDesignStoreConfigurator> _logger;

        public GlobalClaptrapDesignStoreConfigurator(
            IGlobalClaptrapDesign globalClaptrapDesign,
            ILogger<GlobalClaptrapDesignStoreConfigurator>? logger = null)
        {
            _globalClaptrapDesign = globalClaptrapDesign;
            _logger = logger ?? LoggerFactoryHolder.Instance.CreateLogger<GlobalClaptrapDesignStoreConfigurator>();
        }

        public void Configure(IClaptrapDesignStore designStore)
        {
            foreach (var claptrapDesign in designStore)
            {
                if (claptrapDesign.EventLoaderFactoryType == null)
                {
                    claptrapDesign.EventLoaderFactoryType = _globalClaptrapDesign.EventLoaderFactoryType;
                    _logger.LogDebug("{type} in {designIdentity} is null, will use {globalType} from global config",
                        nameof(claptrapDesign.EventLoaderFactoryType), claptrapDesign.Identity,
                        _globalClaptrapDesign.EventLoaderFactoryType);
                }

                if (claptrapDesign.EventSaverFactoryType == null)
                {
                    claptrapDesign.EventSaverFactoryType = _globalClaptrapDesign.EventSaverFactoryType;
                    _logger.LogDebug("{type} in {designIdentity} is null, will use {globalType} from global config",
                        nameof(claptrapDesign.EventSaverFactoryType), claptrapDesign.Identity,
                        _globalClaptrapDesign.EventSaverFactoryType);
                }

                if (claptrapDesign.StateLoaderFactoryType == null)
                {
                    claptrapDesign.StateLoaderFactoryType = _globalClaptrapDesign.StateLoaderFactoryType;
                    _logger.LogDebug("{type} in {designIdentity} is null, will use {globalType} from global config",
                        nameof(claptrapDesign.StateLoaderFactoryType), claptrapDesign.Identity,
                        _globalClaptrapDesign.StateLoaderFactoryType);
                }

                if (claptrapDesign.StateSaverFactoryType == null)
                {
                    claptrapDesign.StateSaverFactoryType = _globalClaptrapDesign.StateSaverFactoryType;
                    _logger.LogDebug("{type} in {designIdentity} is null, will use {globalType} from global config",
                        nameof(claptrapDesign.StateSaverFactoryType), claptrapDesign.Identity,
                        _globalClaptrapDesign.StateSaverFactoryType);
                }

                if (claptrapDesign.InitialStateDataFactoryType == null)
                {
                    claptrapDesign.InitialStateDataFactoryType = _globalClaptrapDesign.InitialStateDataFactoryType;
                    _logger.LogDebug("{type} in {designIdentity} is null, will use {globalType} from global config",
                        nameof(claptrapDesign.InitialStateDataFactoryType), claptrapDesign.Identity,
                        _globalClaptrapDesign.InitialStateDataFactoryType);
                }

                if (claptrapDesign.StateHolderFactoryType == null)
                {
                    claptrapDesign.StateHolderFactoryType = _globalClaptrapDesign.StateHolderFactoryType;
                    _logger.LogDebug("{type} in {designIdentity} is null, will use {globalType} from global config",
                        nameof(claptrapDesign.StateHolderFactoryType), claptrapDesign.Identity,
                        _globalClaptrapDesign.StateHolderFactoryType);
                }

                if (claptrapDesign.StateOptions == null)
                {
                    claptrapDesign.StateOptions = _globalClaptrapDesign.StateOptions;
                    _logger.LogDebug("{type} in {designIdentity} is null, will use {globalType} from global config",
                        nameof(claptrapDesign.StateOptions), claptrapDesign.Identity,
                        _globalClaptrapDesign.StateOptions);
                }
            }
        }
    }
}