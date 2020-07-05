using System.Threading.Tasks;

namespace Newbe.Claptrap
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