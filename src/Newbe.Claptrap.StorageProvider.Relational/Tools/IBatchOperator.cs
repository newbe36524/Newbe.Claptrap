using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.Tools
{
    public interface IBatchOperator<in T> :
        IBatchOperator
    {
        Task CreateTask(T input);
    }

    public interface IBatchOperator
    {
    }
}