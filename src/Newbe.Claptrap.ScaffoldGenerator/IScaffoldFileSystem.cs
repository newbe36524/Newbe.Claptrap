using System.Threading.Tasks;

namespace Newbe.Claptrap.ScaffoldGenerator
{
    public interface IScaffoldFileSystem
    {
        Task SaveFile(string path, string content);
        Task RemoveAll();
    }
}