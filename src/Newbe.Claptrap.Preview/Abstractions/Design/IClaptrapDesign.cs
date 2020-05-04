using System;
using System.Collections.Generic;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Abstractions.Design
{
    public interface IClaptrapDesign
    {
        /// <summary>
        /// what claptrap this design for
        /// </summary>
        IClaptrapIdentity Identity { get; }

        Type StateDataType { get; }
        Type EventLoaderFactoryType { get; set; }
        Type EventSaverFactoryType { get; set; }
        Type StateLoaderFactoryType { get; set; }
        Type StateSaverFactoryType { get; set; }
        Type InitialStateDataFactoryType { get; set; }
        Type StateHolderFactoryType { get; set; }
        StateOptions StateOptions { get; set; }
        Type EventHandlerFactoryFactoryType { get; }
        IReadOnlyDictionary<string, IClaptrapEventHandlerDesign> EventHandlerDesigns { get; }
    }
}