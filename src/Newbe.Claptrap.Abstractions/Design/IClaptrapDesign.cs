using System;
using System.Collections.Generic;

namespace Newbe.Claptrap
{
    public interface IClaptrapDesign
    {
        /// <summary>
        /// What claptrap this design for.
        /// </summary>
        string ClaptrapTypeCode { get; }

        /// <summary>
        /// Design of this claptrap master.
        /// Not null if this claptrap is a minion of other one claptrap.
        /// Null is this is a master claptrap.
        /// </summary>
        IClaptrapDesign ClaptrapMasterDesign { get; }

        /// <summary>
        /// State data type of this claptrap
        /// </summary>
        Type StateDataType { get; }

        /// <summary>
        /// Factory type for <see cref="IEventLoader"/>. It must implement <see cref="IClaptrapComponentFactory{T}"/>
        /// </summary>
        Type EventLoaderFactoryType { get; set; }

        /// <summary>
        /// Factory type for <see cref="IEventSaver"/>. It must implement <see cref="IClaptrapComponentFactory{T}"/>
        /// </summary>
        Type EventSaverFactoryType { get; set; }

        /// <summary>
        /// Factory type for <see cref="IStateLoader"/>. It must implement <see cref="IClaptrapComponentFactory{T}"/>
        /// </summary>
        Type StateLoaderFactoryType { get; set; }

        /// <summary>
        /// Factory type for <see cref="IStateSaver"/>. It must implement <see cref="IClaptrapComponentFactory{T}"/>
        /// </summary>
        Type StateSaverFactoryType { get; set; }

        /// <summary>
        /// Factory type for initial state data. It must implement <see cref="IInitialStateDataFactory"/>
        /// </summary>
        Type InitialStateDataFactoryType { get; set; }

        /// <summary>
        /// Factory type for <see cref="IStateHolder"/>. It must implement <see cref="IClaptrapComponentFactory{T}"/>
        /// </summary>
        Type StateHolderFactoryType { get; set; }

        /// <summary>
        /// Options about claptrap
        /// </summary>
        ClaptrapOptions ClaptrapOptions { get; }

        /// <summary>
        /// Factory type for <see cref="IEventHandlerFactory"/>. It must implement <see cref="IClaptrapComponentFactory{T}"/>
        /// </summary>
        Type EventHandlerFactoryFactoryType { get; set; }

        /// <summary>
        /// Event handler designs for how to handle event
        /// </summary>
        IReadOnlyDictionary<string, IClaptrapEventHandlerDesign> EventHandlerDesigns { get; }

        /// <summary>
        /// Interface type of claptrap box
        /// </summary>
        Type ClaptrapBoxInterfaceType { get; }

        /// <summary>
        /// Implementation type of claptrap box
        /// </summary>
        Type ClaptrapBoxImplementationType { get; }

        ClaptrapStorageProviderOptions ClaptrapStorageProviderOptions { get; }
    }

    public class ClaptrapStorageProviderOptions
    {
        public IEventSaverOptions EventSaverOptions { get; set; }
        public IEventLoaderOptions EventLoaderOptions { get; set; }
        public IStateSaverOptions StateSaverOptions { get; set; }
        public IStateLoaderOptions StateLoaderOptions { get; set; }
    }

    public interface IStorageProviderOptions
    {
    }

    public interface IEventSaverOptions : IStorageProviderOptions
    {
    }

    public class EmptyEventSaverOptions : IEventSaverOptions
    {
    }

    public interface IEventLoaderOptions : IStorageProviderOptions
    {
    }

    public class EmptyEventLoaderOptions : IEventLoaderOptions
    {
    }

    public interface IStateSaverOptions : IStorageProviderOptions
    {
    }

    public class EmptyStateSaverOptions : IStateSaverOptions
    {
    }

    public interface IStateLoaderOptions : IStorageProviderOptions
    {
    }

    public class EmptyStateLoaderOptions : IStateLoaderOptions
    {
    }
}