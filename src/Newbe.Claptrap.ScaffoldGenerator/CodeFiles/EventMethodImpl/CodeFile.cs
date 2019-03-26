namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.EventMethodImpl
{
    public class CodeFile : ICodeFile
    {
        public string EventDataTypeFullName { get; set; }
        public string StateDataTypeFullName { get; set; }
        public string ClassName { get; set; }
        public string InterfaceName { get; set; }
        public string UnwrapTaskReturnTypeName { get; set; }
        public string[] ArgumentTypeAndNames { get; set; }
    }
}