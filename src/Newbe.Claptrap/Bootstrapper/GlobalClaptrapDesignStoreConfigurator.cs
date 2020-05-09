using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Design;
using Newbe.Claptrap.Options;

namespace Newbe.Claptrap.Bootstrapper
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
                foreach (var propertyInfo in DesignProperties())
                {
                    SetValueIfNull(propertyInfo,
                        _globalClaptrapDesign,
                        claptrapDesign,
                        claptrapDesign.Identity);
                }

                if (_globalClaptrapDesign.ClaptrapOptions != null)
                {
                    foreach (var propertyInfo in OptionProperties())
                    {
                        SetValueIfNull(propertyInfo,
                            _globalClaptrapDesign.ClaptrapOptions,
                            claptrapDesign.ClaptrapOptions,
                            claptrapDesign.Identity);
                    }

                    // minion claptrap design
                    if (claptrapDesign.ClaptrapMasterDesign != null)
                    {
                        SetValueIfNull(typeof(ClaptrapOptions).GetProperty(nameof(ClaptrapOptions.MinionOptions)),
                            _globalClaptrapDesign.ClaptrapOptions,
                            claptrapDesign.ClaptrapOptions,
                            claptrapDesign.Identity);
                    }
                }
            }

            void SetValueIfNull(PropertyInfo propertyInfo, object source, object target, IClaptrapIdentity identity)
            {
                var sourceData = source.GetType()
                    .GetProperty(propertyInfo.Name)
                    .GetValue(source);
                var targetData = propertyInfo.GetValue(target);
                if (targetData == null && sourceData != null)
                {
                    propertyInfo.SetValue(target, sourceData);
                    _logger.LogDebug("{type} in {designIdentity} is null, will use {globalType} from global config",
                        propertyInfo.Name,
                        identity,
                        sourceData);
                }
            }

            static IEnumerable<PropertyInfo> DesignProperties()
            {
                var type = typeof(IClaptrapDesign);
                yield return type.GetProperty(nameof(IClaptrapDesign.EventLoaderFactoryType));
                yield return type.GetProperty(nameof(IClaptrapDesign.EventSaverFactoryType));
                yield return type.GetProperty(nameof(IClaptrapDesign.StateLoaderFactoryType));
                yield return type.GetProperty(nameof(IClaptrapDesign.StateSaverFactoryType));
                yield return type.GetProperty(nameof(IClaptrapDesign.InitialStateDataFactoryType));
                yield return type.GetProperty(nameof(IClaptrapDesign.StateHolderFactoryType));
                yield return type.GetProperty(nameof(IClaptrapDesign.ClaptrapOptions));
                yield return type.GetProperty(nameof(IClaptrapDesign.EventHandlerFactoryFactoryType));
            }

            static IEnumerable<PropertyInfo> OptionProperties()
            {
                var type = typeof(ClaptrapOptions);
                yield return type.GetProperty(nameof(ClaptrapOptions.EventLoadingOptions));
                yield return type.GetProperty(nameof(ClaptrapOptions.StateRecoveryOptions));
                yield return type.GetProperty(nameof(ClaptrapOptions.StateSavingOptions));
            }
        }
    }
}