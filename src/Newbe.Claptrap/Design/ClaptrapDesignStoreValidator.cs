using System;
using System.Collections.Generic;
using System.Linq;
using Newbe.Claptrap.Extensions;
using Newbe.Claptrap.Localization;
using SmartFormat;

namespace Newbe.Claptrap.Design
{
    public class ClaptrapDesignStoreValidator : IClaptrapDesignStoreValidator
    {
        private readonly IL _l;

        public ClaptrapDesignStoreValidator(
            IL l)
        {
            _l = l;
        }

        public (bool isOk, string errorMessage) Validate(IEnumerable<IClaptrapDesign> designs)
        {
            var errors = designs.SelectMany(ValidateOne)
                .Where(x => !string.IsNullOrEmpty(x))
                .ToArray();
            var errorMessage = string.Join(",", errors);

            return (!errors.Any(), errorMessage);

            IEnumerable<string> ValidateOne(IClaptrapDesign design)
            {
                foreach (var error in ValidateTypes())
                {
                    if (!string.IsNullOrEmpty(error))
                    {
                        yield return $"{design.ClaptrapTypeCode} {error}";
                    }
                }

                IEnumerable<string> ValidateTypes()
                {
                    foreach (var error in ValidateMasterDesign(design))
                    {
                        yield return error;
                    }

                    yield return ValidateTypeNotNull(design.ClaptrapTypeCode,
                        nameof(design.ClaptrapTypeCode));
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
                    yield return ValidateTypeNotNull(design.ClaptrapOptions,
                        nameof(design.ClaptrapOptions));
                    if (design.ClaptrapOptions != null)
                    {
                        yield return ValidateTypeNotNull(design.ClaptrapOptions.EventLoadingOptions,
                            nameof(design.ClaptrapOptions.EventLoadingOptions));
                        yield return ValidateTypeNotNull(design.ClaptrapOptions.StateRecoveryOptions,
                            nameof(design.ClaptrapOptions.StateRecoveryOptions));
                        yield return ValidateTypeNotNull(design.ClaptrapOptions.StateSavingOptions,
                            nameof(design.ClaptrapOptions.StateSavingOptions));
                        yield return ValidateTypeNotNull(design.ClaptrapOptions.MinionActivationOptions!,
                            nameof(design.ClaptrapOptions.MinionActivationOptions));
                    }

                    yield return ValidateTypeNotNull(design.EventHandlerFactoryFactoryType,
                        nameof(design.EventHandlerFactoryFactoryType));
                    yield return ValidateTypeNotNull(design.EventHandlerDesigns, nameof(design.EventHandlerDesigns));
                    yield return ValidateClaptrapComponent<IEventLoader>(design.EventLoaderFactoryType,
                        nameof(design.EventLoaderFactoryType));
                    yield return ValidateClaptrapComponent<IEventSaver>(design.EventSaverFactoryType,
                        nameof(design.EventSaverFactoryType));
                    yield return ValidateClaptrapComponent<IStateLoader>(design.StateLoaderFactoryType,
                        nameof(design.StateLoaderFactoryType));
                    yield return ValidateClaptrapComponent<IStateSaver>(design.StateSaverFactoryType,
                        nameof(design.StateSaverFactoryType));
                    yield return ValidateClaptrapComponent<IStateHolder>(design.StateHolderFactoryType,
                        nameof(design.StateHolderFactoryType));
                    yield return ValidateClaptrapComponent<IEventHandlerFactory>(design.EventHandlerFactoryFactoryType,
                        nameof(design.EventHandlerFactoryFactoryType));

                    IEnumerable<string> ValidateMasterDesign(IClaptrapDesign minionDesign)
                    {
                        if (!minionDesign.IsMinion())
                        {
                            yield break;
                        }

                        var masterDesign = minionDesign.ClaptrapMasterDesign;
                        foreach (var (key, _) in masterDesign.EventHandlerDesigns)
                        {
                            if (!minionDesign.EventHandlerDesigns.TryGetValue(key, out var handlerDesign))
                            {
                                yield return Smart.Format(_l[LK.MissingEventHandler], new
                                {
                                    eventTypeCode = key,
                                    claptrapTypeCode = minionDesign.ClaptrapTypeCode,
                                    handlerName = nameof(EmptyEventHandler)
                                });
                            }

                            yield return ValidateTypeNotNull(handlerDesign.EventHandlerType,
                                $"{handlerDesign.EventTypeCode} -> {nameof(handlerDesign.EventHandlerType)}");
                        }
                    }

                    string ValidateClaptrapComponent<TComponent>(Type type, string name)
                    {
                        return type == null
                            ? Smart.Format(_l[LK._name__is_required__please_set_it_correctly_], new {name})
                            : type.GetInterface(typeof(TComponent).FullName) != null
                                ? Smart.Format(_l[LK.Type__type__does_not_implement__componentType__],
                                    new {type, componentType = typeof(TComponent)})
                                : string.Empty;
                    }

                    string ValidateTypeNotNull(object type, string name)
                    {
                        return type == null
                            ? Smart.Format(_l![LK._name__is_required__please_set_it_correctly_], new {name})
                            : string.Empty;
                    }
                }
            }
        }
    }
}