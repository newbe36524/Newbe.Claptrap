using System;
using System.Collections.Generic;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Metadata
{
    public class MinionMetadata
    {
        public IMinionKind MinionKind { get; set; }
        public ClaptrapMetadata ClaptrapMetadata { get; set; }

        /// <summary>
        /// events that minion will handled from claptrap
        /// </summary>
        public IEnumerable<ClaptrapEventMetadata> ClaptrapEventMetadata { get; set; }

        public Type StateDataType { get; set; }
        public Type InterfaceType { get; set; }
    }
}