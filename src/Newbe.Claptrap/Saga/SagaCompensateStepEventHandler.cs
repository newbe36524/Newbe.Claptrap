using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Saga
{
    public class SagaCompensateStepEventHandler :
        NormalEventHandler<ISagaStateData, SagaCompensateStepEvent>
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger<SagaCompensateStepEventHandler> _logger;

        public SagaCompensateStepEventHandler(
            ILifetimeScope lifetimeScope,
            ILogger<SagaCompensateStepEventHandler> logger)
        {
            _lifetimeScope = lifetimeScope;
            _logger = logger;
        }

        public override async ValueTask HandleEvent(ISagaStateData stateData,
            SagaCompensateStepEvent eventData,
            IEventContext eventContext)
        {
            var stepIndex = eventData.StepIndex;
            var flowState = stateData.SagaFlowState;
            var stepType = flowState.CompensateSteps[stepIndex];
            var step = (ISagaStep) _lifetimeScope.Resolve(stepType);
            try
            {
                await step.RunAsync(stepIndex, flowState, stateData.GetUserData());
                flowState.CompensateStepStatuses[stepIndex] = StepStatus.Completed;
                if (stepIndex == flowState.LastErrorStepIndex)
                {
                    _logger.LogInformation("Saga flow has been reverted");
                    flowState.IsCompensated = true;
                    flowState.IsCompensating = false;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to compensate step");
                flowState.CompensateStepStatuses[stepIndex] = StepStatus.Error;
                throw;
            }
        }
    }
}