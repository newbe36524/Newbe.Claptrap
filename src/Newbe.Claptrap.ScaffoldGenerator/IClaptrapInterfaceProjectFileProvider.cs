using System.Collections.Generic;
using System.IO;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public interface IClaptrapInterfaceProjectFileProvider
    {
        IEnumerable<FileInfo> GetAllFiles();
    }
}