using System;

namespace Newbe.Claptrap.Preview.Core
{
    /// <inheritdoc />
    /// <summary>
    ///     identity of a actor
    /// </summary>
    public interface IActorIdentity : IEquatable<IActorIdentity>
    {
        /// <summary>
        /// id of a actor.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// type code of actor
        /// </summary>
        string TypeCode { get; }
    }
}