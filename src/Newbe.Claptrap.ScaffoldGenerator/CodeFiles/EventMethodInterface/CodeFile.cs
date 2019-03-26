namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.EventMethodInterface
{
    public class CodeFile : ICodeFile
    {
        public string EventDataFullName { get; set; }
        public string StateDataFullName { get; set; }
        public string InterfaceName { get; set; }
        public string UnwrapTaskReturnTypeName { get; set; }
        public string[] ArgumentTypeAndNames { get; set; }
    }
}