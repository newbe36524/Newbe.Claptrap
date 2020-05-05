using System;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl.Bootstrapper
{
    public interface IGlobalClaptrapDesign
    {
        Type EventLoaderFactoryType { get; set; }
        Type EventSaverFactoryType { get; set; }
        Type StateLoaderFactoryType { get; set; }
        Type StateSaverFactoryType { get; set; }
        Type InitialStateDataFactoryType { get; set; }
        Type StateHolderFactoryType { get; set; }
        StateOptions StateOptions { get; set; }
        Type EventHandlerFactoryFactoryType { get; set; }
    }
}