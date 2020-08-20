using System.Linq;
using System.Threading.Tasks;

namespace Newbe.Claptrap.Saga
{
    public class SagaFlowCreateEventHandler :
        NormalEventHandler<SagaStateData, SagaFlowCreateEvent>
    {
        public override ValueTask HandleEvent(SagaStateData stateData,
            SagaFlowCreateEvent eventData,
            IEventContext eventContext)
        {
            var flowState = new SagaFlowState
            {
                Steps = eventData.Steps.ToArray(),
                StepStatuses = Enumerable
                    .Repeat(StepStatus.NotStarted, eventData.Steps.Count)
                    .ToArray(),
                CompensateSteps = eventData.CompensateSteps.ToArray(),
                CompensateStepStatuses = Enumerable
                    .Repeat(StepStatus.NotStarted, eventData.CompensateSteps.Count)
                    .ToArray(),
                IsCompensated = false,
                IsCompensating = false,
                IsCompleted = false,
            };
            stateData.SagaFlowState = flowState;
            stateData.UserData = eventData.UserData;
            return new ValueTask();
        }
    }
}