using System.Collections.Generic;

namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.ClaptrapGrainNoneEventMethodPart
{
    public class CodeFile : ICodeFile
    {
        public string ClassName { get; set; }
        public IEnumerable<NoneEventMethod> NoneEventMethods { get; set; }
    }
}