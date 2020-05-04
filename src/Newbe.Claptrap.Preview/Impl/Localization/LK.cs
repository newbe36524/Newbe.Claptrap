// ReSharper disable UnusedAutoPropertyAccessor.Global

#pragma warning disable 8618

namespace Newbe.Claptrap.Preview.Impl.Localization
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
            /// start to scan {assemblyArrayCount} assemblies, {assemblyNames}
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

        #region L0003EmptyEventHandledNotifier

        public static class L0003EmptyEventHandledNotifier
        {
            public const string Prefix = nameof(LK) + ".L0003.";

            /// <summary>
            /// Event handled notification received at empty notifier. identity {identity}, event version: {version}, event type code : {eventTypeCode}
            /// </summary>
            public static string L001EventHandled { get; internal set; }
        }

        #endregion

        #region L0004EventHandledNotificationFlow

        public static class L0004EventHandledNotificationFlow
        {
            public const string Prefix = nameof(LK) + ".L0004.";

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
            public const string Prefix = nameof(LK) + ".L0005.";

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
    }
}