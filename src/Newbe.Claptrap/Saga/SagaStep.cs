using System.Threading.Tasks;

namespace Newbe.Claptrap.Saga
{
    public abstract class SagaStep<TUserData> : ISagaStep
    {
        public abstract Task RunAsync(SagaStepData<TUserData> stepData);

        public Task RunAsync(SagaStepData data)
        {
            var sagaStepData =
                new SagaStepData<TUserData>(data.StepIndex, data.FlowState, (TUserData) data.UserData!, data.FlowId);
            return RunAsync(sagaStepData);
        }
    }
}