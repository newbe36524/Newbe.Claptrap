using System;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Saga
{
    public class SagaMoveToNextEventHandler :
        NormalEventHandler<ISagaStateData, SagaMoveToNextEvent>
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger<SagaMoveToNextEventHandler> _logger;

        public SagaMoveToNextEventHandler(
            ILifetimeScope lifetimeScope,
            ILogger<SagaMoveToNextEventHandler> logger)
        {
            _lifetimeScope = lifetimeScope;
            _logger = logger;
        }

        public override async ValueTask HandleEvent(ISagaStateData stateData,
            SagaMoveToNextEvent eventData,
            IEventContext eventContext)
        {
            var stepIndex = eventData.StepIndex;
            var flowState = stateData.SagaFlowState;
            var stepType = flowState.Steps[stepIndex];
            var step = (ISagaStep) _lifetimeScope.Resolve(stepType);
            try
            {
                var stepData = new SagaStepData(stepIndex,
                    flowState,
                    stateData.GetUserData(),
                    eventContext.State.Identity.Id);
                await step.RunAsync(stepData);
                flowState.StepStatuses[stepIndex] = StepStatus.Completed;
                if (stepIndex == flowState.Steps.Length - 1)
                {
                    _logger.LogInformation("final step of saga flow has completed");
                    flowState.IsCompleted = true;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "failed to run a step");
                flowState.StepStatuses[stepIndex] = StepStatus.Error;
                flowState.IsCompensating = true;
                flowState.LastErrorStepIndex = stepIndex;
            }
        }
    }
}