using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Core;
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
                foreach (var propertyInfo in PropertyInfos())
                {
                    SetValueIfNull(propertyInfo, _globalClaptrapDesign, claptrapDesign, claptrapDesign.Identity);
                }
            }

            void SetValueIfNull(PropertyInfo propertyInfo, object source, object target, IClaptrapIdentity identity)
            {
                var sourceData = typeof(IGlobalClaptrapDesign).GetProperty(propertyInfo.Name).GetValue(source);
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

            static IEnumerable<PropertyInfo> PropertyInfos()
            {
                var type = typeof(IClaptrapDesign);
                yield return type.GetProperty(nameof(IClaptrapDesign.EventLoaderFactoryType));
                yield return type.GetProperty(nameof(IClaptrapDesign.EventSaverFactoryType));
                yield return type.GetProperty(nameof(IClaptrapDesign.StateLoaderFactoryType));
                yield return type.GetProperty(nameof(IClaptrapDesign.StateSaverFactoryType));
                yield return type.GetProperty(nameof(IClaptrapDesign.InitialStateDataFactoryType));
                yield return type.GetProperty(nameof(IClaptrapDesign.StateHolderFactoryType));
                yield return type.GetProperty(nameof(IClaptrapDesign.StateOptions));
                yield return type.GetProperty(nameof(IClaptrapDesign.EventHandlerFactoryFactoryType));
            }
        }
    }
}