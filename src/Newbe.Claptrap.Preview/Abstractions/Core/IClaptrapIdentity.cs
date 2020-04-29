using System;

namespace Newbe.Claptrap.Preview.Abstractions.Core
{
    /// <inheritdoc />
    /// <summary>
    ///     identity of a actor
    /// </summary>
    public interface IClaptrapIdentity : IEquatable<IClaptrapIdentity>
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