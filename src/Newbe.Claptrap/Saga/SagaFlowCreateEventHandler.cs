using System.Linq;
using System.Threading.Tasks;

namespace Newbe.Claptrap.Saga
{
    public class SagaFlowCreateEventHandler : NormalEventHandler<ISagaStateData, SagaFlowCreateEvent>
    {
        private readonly ISagaUserDataSerializer _sagaUserDataSerializer;

        public SagaFlowCreateEventHandler(
            ISagaUserDataSerializer sagaUserDataSerializer)
        {
            _sagaUserDataSerializer = sagaUserDataSerializer;
        }

        public override ValueTask HandleEvent(ISagaStateData stateData,
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
                IsCompleted = false
            };
            stateData.SagaFlowState = flowState;
            var userData = _sagaUserDataSerializer.Deserialize(eventData.UserData, eventData.UserDataType);
            stateData.SetUserData(userData);
            return new ValueTask();
        }
    }
}