using System;
using System.Collections.Generic;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Metadata
{
    public class ClaptrapMetadata
    {
        public IClaptrapKind ClaptrapKind { get; set; }
        public IEnumerable<MinionMetadata> MinionMetadata { get; set; }
        public IEnumerable<ClaptrapEventMetadata> ClaptrapEventMetadata { get; set; }
        public Type StateDataType { get; set; }
        public Type InterfaceType { get; set; }
    }
}