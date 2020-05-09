using System;
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
                .Where(filePath => filterFilter(Path.GetFileName(filePath)))
                .ToArray();
            foreach (var file in files)
            {
                Assembly.LoadFrom(file);
            }
        }
    }
}