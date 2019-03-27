using System.Collections.Generic;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE02ClaptrapGrainNoneEventMethodPart
{
    public class GE02CodeFile : ICodeFile
    {
        public string ClassName { get; set; }
        public IEnumerable<NoneEventMethod> NoneEventMethods { get; set; }
        public string FileName { get; set; }
    }
}