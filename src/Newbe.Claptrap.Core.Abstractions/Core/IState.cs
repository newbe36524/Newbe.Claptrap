namespace Newbe.Claptrap.Core
{
    public interface IState
    {
        IActorIdentity Identity { get; }
        IStateData Data { get; }

        /// <summary>
        /// current version of state, this will be increased if a event has been processed successfully.
        /// this is the same as last event version while has been processed successfully.
        /// </summary>
        ulong Version { get; }

        void IncreaseVersion();
    }
}