using System.Threading.Tasks;

namespace Newbe.Claptrap
{
    public interface IBatchOperator<in T> :
        IBatchOperator
    {
        ValueTask CreateTask(T input);
    }

    public interface IBatchOperator
    {
    }
}