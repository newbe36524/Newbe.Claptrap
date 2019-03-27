namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE04EventMethodInterface
{
    public class GE04CodeFile : ICodeFile
    {
        public string[] Namespaces { get; set; }
        public string EventDataFullName { get; set; }
        public string StateDataFullName { get; set; }
        public string InterfaceName { get; set; }
        public string UnwrapTaskReturnTypeName { get; set; }
        public string[] ArgumentTypeAndNames { get; set; }
        public string FileName { get; set; }
    }
}