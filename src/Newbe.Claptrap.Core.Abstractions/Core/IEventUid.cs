using System;

namespace Newbe.Claptrap.Core
{
    /// <inheritdoc />
    /// <summary>
    ///  unique id of event, events with the same uid will be process only once.
    /// </summary>
    public interface IEventUid : IEquatable<IEventUid>
    {
    }
}