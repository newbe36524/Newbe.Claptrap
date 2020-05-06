using System;
using System.Collections.Generic;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Options;

namespace Newbe.Claptrap.Preview.Abstractions.Design
{
    public interface IClaptrapDesign
    {
        /// <summary>
        /// what claptrap this design for
        /// </summary>
        IClaptrapIdentity Identity { get; }

        /// <summary>
        /// Design of this claptrap master.
        /// Not null if this claptrap is a minion of other one claptrap.
        /// Null is this is a master claptrap.
        /// </summary>
        IClaptrapDesign ClaptrapMasterDesign { get; }

        Type StateDataType { get; }
        Type EventLoaderFactoryType { get; set; }
        Type EventSaverFactoryType { get; set; }
        Type StateLoaderFactoryType { get; set; }
        Type StateSaverFactoryType { get; set; }
        Type InitialStateDataFactoryType { get; set; }
        Type StateHolderFactoryType { get; set; }
        ClaptrapOptions ClaptrapOptions { get; set; }
        Type EventHandlerFactoryFactoryType { get; set; }
        IReadOnlyDictionary<string, IClaptrapEventHandlerDesign> EventHandlerDesigns { get; }

        Type ClaptrapBoxInterfaceType { get; }
        Type ClaptrapBoxImplementationType { get; }
    }
}