using System;
using System.Text;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public static class StringBuilderExtensions
    {
        public static void AppendSummaryComments(this StringBuilder sb, string summary)
        {
            sb.AppendLine("/// <summary>");
            sb.AppendLine($"/// {summary}");
            sb.AppendLine("/// </summary>");
        }

        public static void UsingCurlyBraces(this StringBuilder sb, Action action)
        {
            sb.AppendLine("{");
            action();
            sb.AppendLine("}");
        }

        public static void AppendNotImplemented(this StringBuilder sb)
        {
            sb.AppendLine("// TODO please add your code here and remove the exception");
            sb.AppendLine("throw new NotImplementedException();");
        }
    }
}