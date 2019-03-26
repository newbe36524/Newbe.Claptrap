using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.StateDataUpdater
{
    public class CodeFileGeneratorContext : ICodeFileGeneratorContext
    {
        public ClaptrapEventMetadata ClaptrapEventMetadata { get; set; }
        public ClaptrapMetadata ClaptrapMetadata { get; set; }
    }
}