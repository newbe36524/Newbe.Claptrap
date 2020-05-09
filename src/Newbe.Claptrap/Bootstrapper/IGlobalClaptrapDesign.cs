using System;
using Newbe.Claptrap.Options;

namespace Newbe.Claptrap.Bootstrapper
{
    public interface IGlobalClaptrapDesign
    {
        Type EventLoaderFactoryType { get; set; }
        Type EventSaverFactoryType { get; set; }
        Type StateLoaderFactoryType { get; set; }
        Type StateSaverFactoryType { get; set; }
        Type InitialStateDataFactoryType { get; set; }
        Type StateHolderFactoryType { get; set; }
        ClaptrapOptions ClaptrapOptions { get; set; }
        Type EventHandlerFactoryFactoryType { get; set; }
    }
}