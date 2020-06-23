using System;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap
{
    public static class MethodTimeLogger
    {
        public static ILoggerFactory? LoggerFactory { get; internal set; } = null!;

        public static void Log(MethodBase methodBase, TimeSpan elapsed, string message)
        {
            var logger = LoggerFactory?.CreateLogger(typeof(MethodTimeLogger));
            logger?.LogTrace("{type}.{method} cost {time}", methodBase.DeclaringType, methodBase.Name, elapsed);
        }
    }
}