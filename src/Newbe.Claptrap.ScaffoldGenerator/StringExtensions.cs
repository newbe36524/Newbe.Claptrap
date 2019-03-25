using System;

namespace Newbe.Claptrap.ScaffoldGenerator
{
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