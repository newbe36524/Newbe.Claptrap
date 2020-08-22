using System.Threading.Tasks;

namespace Newbe.Claptrap.Saga
{
    public interface ISagaStep
    {
        Task RunAsync(int stepIndex, SagaFlowState flowState, object userData);
    }
}