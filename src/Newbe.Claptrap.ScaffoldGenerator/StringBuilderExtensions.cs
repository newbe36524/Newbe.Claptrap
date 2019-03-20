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

        public static void AppendNotImplementedException(this StringBuilder sb)
        {
            sb.AppendLine("throw new NotImplementedException();");
        }
    }

    public static class StringExtensions
    {
        public static string GetImplName(this string interfaceName)
        {
            if (string.IsNullOrEmpty(interfaceName))
            {
                throw new ArgumentNullException(nameof(interfaceName));
            }

            if (interfaceName.StartsWith("I"))
            {
                return interfaceName.Substring(1);
            }

            return $"{interfaceName}Impl";
        }
    }
}