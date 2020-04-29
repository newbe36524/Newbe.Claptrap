using System;
using System.Collections.Generic;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Abstractions.Metadata
{
    public interface IClaptrapDesign
    {
        /// <summary>
        /// what claptrap this design for
        /// </summary>
        IClaptrapIdentity Identity { get; }

        Type ActorStateDataType { get; }
        Type EventLoaderFactoryType { get; }
        Type EventSaverFactoryType { get; }
        Type StateLoaderFactoryType { get; }
        Type StateSaverFactoryType { get; }
        Type InitialStateDataFactoryType { get; }
        Type StateHolderFactoryType { get; }
        Type EventHandlerFactoryFactoryType { get; }
        StateSavingOptions StateSavingOptions { get; }
        IReadOnlyDictionary<string, IClaptrapEventHandlerDesign> EventHandlerDesigns { get; }
    }
}