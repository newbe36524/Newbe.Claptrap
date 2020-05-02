using System;
using System.Collections.Generic;
using System.Text;
using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Metadata;

namespace Newbe.Claptrap.Preview.Impl.Metadata
{
    public class ClaptrapDesignStoreValidator : IClaptrapDesignStoreValidator
    {
        public (bool isOk, string errorMessage) Validate(IClaptrapDesignStore claptrapDesignStore)
        {
            var sb = new StringBuilder();
            var isOk = true;
            foreach (var design in claptrapDesignStore)
            {
                if (!ValidateOne(design))
                {
                    isOk = false;
                }
            }

            return (isOk, sb.ToString());

            bool ValidateOne(IClaptrapDesign design)
            {
                var oneIsAllOk = true;
                foreach (var (isComponentOk, errorMessage) in ValidateTypes())
                {
                    if (!isComponentOk)
                    {
                        sb!.AppendLine(errorMessage);
                        oneIsAllOk = false;
                    }
                }

                return oneIsAllOk;

                IEnumerable<(bool isOk, string errorMessage)> ValidateTypes()
                {
                    yield return ValidateTypeNotNull(design.Identity,
                        nameof(design.Identity));
                    yield return ValidateTypeNotNull(design.StateDataType,
                        nameof(design.StateDataType));
                    yield return ValidateTypeNotNull(design.EventLoaderFactoryType,
                        nameof(design.EventLoaderFactoryType));
                    yield return ValidateTypeNotNull(design.EventSaverFactoryType,
                        nameof(design.EventSaverFactoryType));
                    yield return ValidateTypeNotNull(design.StateLoaderFactoryType,
                        nameof(design.StateLoaderFactoryType));
                    yield return ValidateTypeNotNull(design.StateSaverFactoryType,
                        nameof(design.StateSaverFactoryType));
                    yield return ValidateTypeNotNull(design.InitialStateDataFactoryType,
                        nameof(design.InitialStateDataFactoryType));
                    yield return ValidateTypeNotNull(design.StateHolderFactoryType,
                        nameof(design.StateHolderFactoryType));
                    yield return ValidateTypeNotNull(design.StateOptions, nameof(design.StateOptions));
                    yield return ValidateTypeNotNull(design.EventHandlerFactoryFactoryType,
                        nameof(design.EventHandlerFactoryFactoryType));
                    yield return ValidateTypeNotNull(design.EventHandlerDesigns, nameof(design.EventHandlerDesigns));
                    yield return ValidateClaptrapComponent<IEventLoader>(design.EventLoaderFactoryType);
                    yield return ValidateClaptrapComponent<IEventSaver>(design.EventSaverFactoryType);
                    yield return ValidateClaptrapComponent<IStateLoader>(design.StateLoaderFactoryType);
                    yield return ValidateClaptrapComponent<IStateSaver>(design.StateSaverFactoryType);
                    yield return ValidateClaptrapComponent<IStateHolder>(design.StateHolderFactoryType);
                    yield return ValidateClaptrapComponent<IEventHandlerFactory>(design.EventHandlerFactoryFactoryType);

                    static (bool isOk, string errorMessage) ValidateClaptrapComponent<TComponent>(Type type)
                    {
                        return type.GetInterface(typeof(TComponent).FullName) != null
                            ? (false, $"type {type} is not implement {typeof(TComponent)} .")
                            : (true, string.Empty);
                    }

                    static (bool isOk, string errorMessage) ValidateTypeNotNull(object type, string name)
                    {
                        return type == null
                            ? (false, $"{name} is required, please set it correctly")
                            : (true, string.Empty);
                    }
                }
            }
        }
    }
}