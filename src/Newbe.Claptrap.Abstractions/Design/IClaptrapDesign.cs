using System;
using System.Collections.Generic;

namespace Newbe.Claptrap
{
    public interface IClaptrapDesign
    {
        /// <summary>
        /// What claptrap this design for.
        /// If Id is null, it means that is for all claptrap with this TypeCode.
        /// If Id is not null, it means for the specified Id claptrap
        /// </summary>
        IClaptrapIdentity Identity { get; }

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
        ClaptrapOptions ClaptrapOptions { get; set; }

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

        StorageProviderOptions StorageProviderOptions { get; }
    }

    public class StorageProviderOptions
    {
        public IEventSaverOptions EventSaverOptions { get; }
        public IEventLoaderOptions EventLoaderOptions { get; }
        public IStateSaverOptions StateSaverOptions { get; }
        public IStateLoaderOptions StateLoaderOptions { get; }
    }

    public interface IEventSaverOptions
    {
    }

    public interface IEventLoaderOptions
    {
    }

    public interface IStateSaverOptions
    {
    }

    public interface IStateLoaderOptions
    {
    }
}