namespace Newbe.Claptrap.ScaffoldGenerator.CodeFiles.GE03EventMethodImpl
{
    public class GE03CodeFile : ICodeFile
    {
        public string EventDataTypeFullName { get; set; }
        public string StateDataTypeFullName { get; set; }
        public string ClassName { get; set; }
        public string InterfaceName { get; set; }
        public string UnwrapTaskReturnTypeName { get; set; }
        public string[] ArgumentTypeAndNames { get; set; }
        public string FileName { get; set; }
    }
}