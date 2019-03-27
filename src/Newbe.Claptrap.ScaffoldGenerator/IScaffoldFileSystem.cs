using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public interface IScaffoldFileSystem
    {
        Task SaveFile(string path, string content);
        Task RemoveAll();
    }

    public class ScaffoldFileSystem : IScaffoldFileSystem
    {
        private readonly string _rootPath;

        public ScaffoldFileSystem(
            string rootPath)
        {
            _rootPath = rootPath;
        }

        public async Task SaveFile(string path, string content)
        {
            var filename = Path.Combine(_rootPath, path);
            var fileInfo = new FileInfo(filename);
            Debug.Assert(fileInfo.Directory != null, "fileInfo.Directory != null");
            if (!fileInfo.Directory.Exists)
            {
                Directory.CreateDirectory(fileInfo.Directory.FullName);
            }

            using (var streamWriter = fileInfo.CreateText())
            {
                await streamWriter.WriteAsync(content);
            }
        }

        public Task RemoveAll()
        {
            var directories = Directory.GetDirectories(_rootPath);
            foreach (var directory in directories)
            {
                Directory.Delete(directory, true);
            }

            var files = Directory.GetFiles(_rootPath, "*.cs");
            foreach (var file in files)
            {
                File.Delete(file);
            }

            return Task.CompletedTask;
        }
    }

    public interface IClaptrapInterfaceProjectFileProvider
    {
        IEnumerable<FileInfo> GetAllFiles();
    }

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