using System;
using System.Collections.Generic;

namespace Newbe.Claptrap
{
    public record ClaptrapDesign : IClaptrapDesign
    {
        public ClaptrapDesign()
        {
        }

        public ClaptrapDesign(
            IClaptrapDesign claptrapDesign)
        {
            ClaptrapTypeCode = claptrapDesign.ClaptrapTypeCode;
            ClaptrapMasterDesign = claptrapDesign.ClaptrapMasterDesign;
            ClaptrapOptions = claptrapDesign.ClaptrapOptions;
            StateDataType = claptrapDesign.StateDataType;
            EventLoaderFactoryType = claptrapDesign.EventLoaderFactoryType;
            EventSaverFactoryType = claptrapDesign.EventSaverFactoryType;
            StateLoaderFactoryType = claptrapDesign.StateLoaderFactoryType;
            StateSaverFactoryType = claptrapDesign.StateSaverFactoryType;
            InitialStateDataFactoryType = claptrapDesign.InitialStateDataFactoryType;
            StateHolderFactoryType = claptrapDesign.StateHolderFactoryType;
            EventHandlerFactoryFactoryType = claptrapDesign.EventHandlerFactoryFactoryType;
            EventNotifierFactoryType = claptrapDesign.EventNotifierFactoryType;
            EventHandlerDesigns = claptrapDesign.EventHandlerDesigns;
            ClaptrapBoxInterfaceType = claptrapDesign.ClaptrapBoxInterfaceType;
            ClaptrapBoxImplementationType = claptrapDesign.ClaptrapBoxImplementationType;
            ExtendInfos = claptrapDesign.ExtendInfos;
            ClaptrapStorageProviderOptions = claptrapDesign.ClaptrapStorageProviderOptions;
        }

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