// ReSharper disable UnusedAutoPropertyAccessor.Global

#pragma warning disable 8618

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Localization key about this project.
    /// You can add a nested static class in this type and localize message in this project.
    /// You can use Newbe.Claptrap.DevTools to generate L*.ini files 
    /// </summary>
    public static partial class LK
    {
        #region L0001AutofacClaptrapBootstrapperBuilder

        public static class L0001AutofacClaptrapBootstrapperBuilder
        {
            public const string Prefix = nameof(LK) + ".L0001.";

            /// <summary>
            /// failed to build claptrap bootstrapper
            /// </summary>
            public static string L001BuildException { get; internal set; }

            /// <summary>
            /// add {provider} as claptrap design provider
            /// </summary>
            public static string L002AddProviderAsClaptrapDesignProvider { get; internal set; }

            /// <summary>
            /// start to scan {assemblyArrayCount} types
            /// </summary>
            public static string L003StartToScan { get; internal set; }

            /// <summary>
            /// start to create claptrap design
            /// </summary>
            public static string L004StartToCreateClaptrapDesign { get; internal set; }

            /// <summary>
            /// claptrap design store created, start to configure it
            /// </summary>
            public static string L005ClaptrapStoreCreated { get; internal set; }

            /// <summary>
            /// all designs : {designs}
            /// </summary>
            public static string L006ShowAllDesign { get; internal set; }

            /// <summary>
            /// start to configure claptrap design store by {configurator}
            /// </summary>
            public static string L007StartToConfigureDesignStore { get; internal set; }

            /// <summary>
            /// found {actorCount} claptrap designs
            /// </summary>
            public static string L008CountDesigns { get; internal set; }

            /// <summary>
            /// all designs after configuration: {designs}
            /// </summary>
            public static string L009ShowDesignsAfterConfiguration { get; internal set; }

            /// <summary>
            /// start to validate all design in claptrap design store
            /// </summary>
            public static string L010StartToValidateDesigns { get; internal set; }

            /// <summary>
            /// all design validated ok
            /// </summary>
            public static string L011DesignValidationSuccess { get; internal set; }
        }

        #endregion

        #region L0002ClaptrapActor

        public static class L0002ClaptrapActor
        {
            public const string Prefix = nameof(LK) + ".L0002.";

            /// <summary>
            /// failed to activate claptrap {identity}
            /// </summary>
            public static string L001FailedToActivate { get; internal set; }
        }

        #endregion

        #region L0004EventHandledNotificationFlow

        public static class L0004EventHandledNotificationFlow
        {
            /// <summary>
            /// success to notify about event be handled. event version : {version}
            /// </summary>
            public static string L001SuccessToNotify { get; internal set; }

            /// <summary>
            /// failed to notify about event be handled. event version : {version}
            /// </summary>
            public static string L002FailToNotify { get; internal set; }
        }

        #endregion

        #region L0005StateRestorer

        public static class L0005StateRestorer
        {
            /// <summary>
            /// there is no state snapshot found from state loader
            /// </summary>
            public static string L001LogThereIsNoStateSnapshot { get; internal set; }

            /// <summary>
            /// found state snapshot from state loader
            /// </summary>
            public static string L002LogStateSnapshotFound { get; internal set; }
        }

        #endregion

        #region L0006ClaptrapFactory

        public static class L0006ClaptrapFactory
        {
            /// <summary>
            /// failed to create a claptrap. {identity}
            /// </summary>
            public static string L001FailedToCreate { get; internal set; }

            /// <summary>
            /// This is a minion claptrap since it contains a master design in it`s design. master type code : {typeCode}
            /// </summary>
            public static string L002MasterFound { get; internal set; }

            /// <summary>
            /// This is a master claptrap. type code : {typeCode}
            /// </summary>
            public static string L003MasterFound { get; internal set; }
        }

        #endregion

        #region L0007ClaptrapDesignStoreValidator

        public static class L0007ClaptrapDesignStoreValidator
        {
            /// <summary>
            /// {name} is required, please set it correctly.
            /// </summary>
            public static string L001ValueCannotBeNull { get; internal set; }

            /// <summary>
            /// Type {type} does not implement {componentType}.
            /// </summary>
            public static string L002NotImpl { get; internal set; }

            /// <summary>
            /// There is no event handler found for {eventTypeCode} in {claptrapTypeCode}. It must be define as this is a minion and the mater will send it to this. If you don`t handle the event, you can define {handlerName} for this event.
            /// </summary>
            public static string L003MissingEventHandleInMinion { get; internal set; }
        }

        #endregion
    }
}