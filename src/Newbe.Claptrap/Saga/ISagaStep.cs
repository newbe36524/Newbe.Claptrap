using System.Threading.Tasks;

namespace Newbe.Claptrap.Saga
{
    public interface ISagaStep
    {
        Task RunAsync(SagaStepData data);
    }
}