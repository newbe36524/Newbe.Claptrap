namespace Newbe.Claptrap.Preview.Abstractions.Core
{
    public interface IState
    {
        IClaptrapIdentity Identity { get; }
        IStateData Data { get; }

        /// <summary>
        /// current version of state, this will be increased if a event has been processed successfully.
        /// this is the same as last event version while has been processed successfully.
        /// </summary>
        long Version { get; }

        long NextVersion => Version + 1;

        void IncreaseVersion();
    }
}