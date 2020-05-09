using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Newbe.Claptrap
{
    public static class AssemblyHelper
    {
        /// <summary>
        /// Get all reference assemblies from root
        /// </summary>
        /// <param name="baseDir"></param>
        /// <param name="filterFilter">continue to get reference or not</param>
        /// <returns></returns>
        public static void ScanAndLoadClaptrapAssemblies(string baseDir,
            Func<string, bool> filterFilter)
        {
            var files = Directory.GetFiles(baseDir, "*.dll")
                .Where(filterFilter)
                .ToArray();
            var assemblies = files
                .Select(Assembly.LoadFile)
                .ToArray();
            foreach (var assembly in assemblies)
            {
                AppDomain.CurrentDomain.Load(assembly.GetName());
            }
        }
    }
}