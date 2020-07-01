using System;
using System.Collections.Generic;

namespace Newbe.Claptrap
{
    public class ClaptrapDesign : IClaptrapDesign
    {
        public string ClaptrapTypeCode { get; set; } = null!;
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
        public Type EventNotifierFactoryType { get; set; } = null!;
        public IReadOnlyDictionary<string, IClaptrapEventHandlerDesign> EventHandlerDesigns { get; set; } = null!;
        public Type ClaptrapBoxInterfaceType { get; set; } = null!;
        public Type ClaptrapBoxImplementationType { get; set; } = null!;

        public ClaptrapStorageProviderOptions ClaptrapStorageProviderOptions { get; } =
            new ClaptrapStorageProviderOptions();

        public IDictionary<string, object> ExtendInfos { get; } = new Dictionary<string, object>();
    }
}