using System;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    internal static class StringExtensions
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

        /// <summary>
        /// remove &lt; and &gt; once from source.
        /// </summary>
        /// <code>
        /// "&lt;&gt;" -> ""
        /// "&lt;int&gt;" -> "int"
        /// "&lt;List&lt;int&gt;&gt;" -> "List&lt;int&gt;"
        /// </code>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string RemoveGtLt(this string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Length < 2)
            {
                throw new ArgumentOutOfRangeException(nameof(source), "length of source must > 2");
            }

            if (source[0] == '<' && source[source.Length - 1] == '>')
            {
                return source.Substring(1, source.Length - 2);
            }

            throw new ArgumentOutOfRangeException(nameof(source), "source string must start with '<' and end with '>'");
        }
    }
}