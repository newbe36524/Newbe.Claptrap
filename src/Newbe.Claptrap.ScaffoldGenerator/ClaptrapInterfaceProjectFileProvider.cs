using System.Collections.Generic;
using System.IO;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public class ClaptrapInterfaceProjectFileProvider : IClaptrapInterfaceProjectFileProvider
    {
        private readonly string _rootPath;

        public ClaptrapInterfaceProjectFileProvider(
            string rootPath)
        {
            _rootPath = rootPath;
        }

        public IEnumerable<FileInfo> GetAllFiles()
        {
            var filenames = Directory.GetFiles(_rootPath, "*.cs", SearchOption.AllDirectories);
            foreach (var filename in filenames)
            {
                yield return new FileInfo(filename);
            }
        }
    }
}