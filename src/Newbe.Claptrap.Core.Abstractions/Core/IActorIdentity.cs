using System;

namespace Newbe.Claptrap.Core
{
    /// <inheritdoc />
    /// <summary>
    ///     identity of a actor
    /// </summary>
    public interface IActorIdentity : IEquatable<IActorIdentity>
    {
        /// <summary>
        ///     id of a actor. it is unique id if the kind is the same.
        /// </summary>
        string Id { get; }
    }
}