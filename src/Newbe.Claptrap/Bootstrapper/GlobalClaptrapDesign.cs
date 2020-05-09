using System;
using Newbe.Claptrap.Options;

namespace Newbe.Claptrap.Bootstrapper
{
    public class GlobalClaptrapDesign : IGlobalClaptrapDesign
    {
        public Type EventLoaderFactoryType { get; set; } = null!;
        public Type EventSaverFactoryType { get; set; } = null!;
        public Type StateLoaderFactoryType { get; set; } = null!;
        public Type StateSaverFactoryType { get; set; } = null!;
        public Type InitialStateDataFactoryType { get; set; } = null!;
        public Type StateHolderFactoryType { get; set; } = null!;
        public ClaptrapOptions ClaptrapOptions { get; set; } = null;
        public Type EventHandlerFactoryFactoryType { get; set; } = null!;
    }
}