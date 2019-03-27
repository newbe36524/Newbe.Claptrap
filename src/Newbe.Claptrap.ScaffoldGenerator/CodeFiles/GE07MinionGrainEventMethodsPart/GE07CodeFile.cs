using System.Collections.Generic;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE07MinionGrainEventMethodsPart
{
    public class GE07CodeFile : ICodeFile
    {
        public string StateDataTypeFullName { get; set; }
        public string ClaptrapCatalog { get; set; }
        public string MinionCatalog { get; set; }
        public string ClassName { get; set; }
        public string InterfaceName { get; set; }
        public IEnumerable<string> EventMethods { get; set; }
        public string FileName { get; set; }
    }
}