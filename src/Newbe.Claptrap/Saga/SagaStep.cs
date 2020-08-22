using System.Threading.Tasks;

namespace Newbe.Claptrap.Saga
{
    public abstract class SagaStep<TUserData> : ISagaStep
    {
        public abstract Task RunAsync(int stepIndex, SagaFlowState flowState, TUserData userData);

        public Task RunAsync(int stepIndex, SagaFlowState flowState, object userData)
        {
            return RunAsync(stepIndex, flowState, (TUserData) userData);
        }
    }
}