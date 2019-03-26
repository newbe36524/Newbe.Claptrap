using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public interface IScaffoldFileSystem
    {
        Task SaveFile(string path, string content);
    }

    public interface IClaptrapInterfaceProjectFileProvider
    {
        IEnumerable<FileInfo> GetAllFiles();
    }
}