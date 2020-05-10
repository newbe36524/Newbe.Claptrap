using System;

namespace Newbe.Claptrap
{
    /// <inheritdoc />
    /// <summary>
    ///     identity of a claptrap
    /// </summary>
    public interface IClaptrapIdentity : IEquatable<IClaptrapIdentity>
    {
        /// <summary>
        /// id of a claptrap.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// type code of claptrap
        /// </summary>
        string TypeCode { get; }
    }
}