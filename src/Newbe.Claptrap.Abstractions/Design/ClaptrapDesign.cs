using System;
using System.Collections.Generic;
using Newbe.Claptrap.Options;

namespace Newbe.Claptrap.Design
{
    public class ClaptrapDesign : IClaptrapDesign
    {
        public IClaptrapIdentity Identity { get; set; } = null!;
        public IClaptrapDesign ClaptrapMasterDesign { get; set; } = null!;
        public ClaptrapOptions ClaptrapOptions { get; set; } = null!;
        public Type StateDataType { get; set; } = null!;
        public Type EventLoaderFactoryType { get; set; } = null!;
        public Type EventSaverFactoryType { get; set; } = null!;
        public Type StateLoaderFactoryType { get; set; } = null!;
        public Type StateSaverFactoryType { get; set; } = null!;
        public Type InitialStateDataFactoryType { get; set; } = null!;
        public Type StateHolderFactoryType { get; set; } = null!;
        public Type EventHandlerFactoryFactoryType { get; set; } = null!;
        public IReadOnlyDictionary<string, IClaptrapEventHandlerDesign> EventHandlerDesigns { get; set; } = null!;
        public Type ClaptrapBoxInterfaceType { get; set; } = null!;
        public Type ClaptrapBoxImplementationType { get; set; } = null!;
    }
}