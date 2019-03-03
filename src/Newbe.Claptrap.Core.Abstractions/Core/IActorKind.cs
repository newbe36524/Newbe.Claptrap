namespace Newbe.Claptrap.Core
{
    /// <summary>
    /// the kind of a actor. actor must be the same if they have the same ActorKind and Id
    /// </summary>
    public interface IActorKind
    {
        /// <summary>
        /// actor type
        /// </summary>
        ActorType ActorType { get; }
    }
}