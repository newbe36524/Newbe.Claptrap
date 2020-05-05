using System;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public class GlobalClaptrapDesign : IGlobalClaptrapDesign
    {
        public Type EventLoaderFactoryType { get; set; } = null!;
        public Type EventSaverFactoryType { get; set; } = null!;
        public Type StateLoaderFactoryType { get; set; } = null!;
        public Type StateSaverFactoryType { get; set; } = null!;
        public Type InitialStateDataFactoryType { get; set; } = null!;
        public Type StateHolderFactoryType { get; set; } = null!;
        public StateOptions StateOptions { get; set; } = null!;
        public Type EventHandlerFactoryFactoryType { get; set; } = null!;
    }
}