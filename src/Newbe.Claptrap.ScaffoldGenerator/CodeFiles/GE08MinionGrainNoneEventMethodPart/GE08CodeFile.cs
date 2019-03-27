using System.Collections.Generic;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE08MinionGrainNoneEventMethodPart
{
    public class GE08CodeFile : ICodeFile
    {
        public string ClassName { get; set; }
        public IEnumerable<NoneEventMethod> NoneEventMethods { get; set; }
        public string FileName { get; set; }
    }
}