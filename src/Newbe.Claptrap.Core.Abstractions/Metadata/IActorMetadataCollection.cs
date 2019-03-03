using System.Collections.Generic;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Metadata
{
    public interface IActorMetadataCollection
    {
        /// <summary>
        /// get claptrap metadata by claptrap kind
        /// </summary>
        /// <param name="claptrapKind"></param>
        /// <exception cref="ActorMetadataNotFoundException"></exception>
        ClaptrapMetadata this[IClaptrapKind claptrapKind] { get; }

        IEnumerable<ClaptrapMetadata> ClaptrapMetadata { get; }

        /// <summary>
        /// get minion metadata by minion kind
        /// </summary>
        /// <param name="minionKind"></param>
        /// <exception cref="ActorMetadataNotFoundException"></exception>
        MinionMetadata this[IMinionKind minionKind] { get; }

        IEnumerable<MinionMetadata> MinionMetadata { get; }
    }
}