using Newbe.Claptrap.Preview.Abstractions.Metadata;
using Newbe.Claptrap.Preview.Logging;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public class GlobalClaptrapDesignStoreConfigurator : IClaptrapDesignStoreConfigurator
    {
        private static readonly ILog Logger = LogProvider.For<GlobalClaptrapDesignStoreConfigurator>();
        private readonly IGlobalClaptrapDesign _globalClaptrapDesign;

        public GlobalClaptrapDesignStoreConfigurator(
            IGlobalClaptrapDesign globalClaptrapDesign)
        {
            _globalClaptrapDesign = globalClaptrapDesign;
        }

        public void Configure(IClaptrapDesignStore designStore)
        {
            foreach (var claptrapDesign in designStore)
            {
                if (claptrapDesign.EventLoaderFactoryType == null)
                {
                    claptrapDesign.EventLoaderFactoryType = _globalClaptrapDesign.EventLoaderFactoryType;
                    Logger.Debug("{type} in {designIdentity} is null, will use {globalType} from global config",
                        nameof(claptrapDesign.EventLoaderFactoryType), claptrapDesign.Identity,
                        _globalClaptrapDesign.EventLoaderFactoryType);
                }

                if (claptrapDesign.EventSaverFactoryType == null)
                {
                    claptrapDesign.EventSaverFactoryType = _globalClaptrapDesign.EventSaverFactoryType;
                    Logger.Debug("{type} in {designIdentity} is null, will use {globalType} from global config",
                        nameof(claptrapDesign.EventSaverFactoryType), claptrapDesign.Identity,
                        _globalClaptrapDesign.EventSaverFactoryType);
                }

                if (claptrapDesign.StateLoaderFactoryType == null)
                {
                    claptrapDesign.StateLoaderFactoryType = _globalClaptrapDesign.StateLoaderFactoryType;
                    Logger.Debug("{type} in {designIdentity} is null, will use {globalType} from global config",
                        nameof(claptrapDesign.StateLoaderFactoryType), claptrapDesign.Identity,
                        _globalClaptrapDesign.StateLoaderFactoryType);
                }

                if (claptrapDesign.StateSaverFactoryType == null)
                {
                    claptrapDesign.StateSaverFactoryType = _globalClaptrapDesign.StateSaverFactoryType;
                    Logger.Debug("{type} in {designIdentity} is null, will use {globalType} from global config",
                        nameof(claptrapDesign.StateSaverFactoryType), claptrapDesign.Identity,
                        _globalClaptrapDesign.StateSaverFactoryType);
                }

                if (claptrapDesign.InitialStateDataFactoryType == null)
                {
                    claptrapDesign.InitialStateDataFactoryType = _globalClaptrapDesign.InitialStateDataFactoryType;
                    Logger.Debug("{type} in {designIdentity} is null, will use {globalType} from global config",
                        nameof(claptrapDesign.InitialStateDataFactoryType), claptrapDesign.Identity,
                        _globalClaptrapDesign.InitialStateDataFactoryType);
                }

                if (claptrapDesign.StateHolderFactoryType == null)
                {
                    claptrapDesign.StateHolderFactoryType = _globalClaptrapDesign.StateHolderFactoryType;
                    Logger.Debug("{type} in {designIdentity} is null, will use {globalType} from global config",
                        nameof(claptrapDesign.StateHolderFactoryType), claptrapDesign.Identity,
                        _globalClaptrapDesign.StateHolderFactoryType);
                }

                if (claptrapDesign.StateOptions == null)
                {
                    claptrapDesign.StateOptions = _globalClaptrapDesign.StateOptions;
                    Logger.Debug("{type} in {designIdentity} is null, will use {globalType} from global config",
                        nameof(claptrapDesign.StateOptions), claptrapDesign.Identity,
                        _globalClaptrapDesign.StateOptions);
                }
            }
        }
    }
}