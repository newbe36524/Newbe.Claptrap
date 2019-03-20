using System.Reflection;

namespace Newbe.Claptrap.Metadata
{
    public class ClaptrapEventMethodMetadata
    {
        public MethodInfo MethodInfo { get; set; }
        public ClaptrapEventMetadata ClaptrapEventMetadata { get; set; }
    }
}