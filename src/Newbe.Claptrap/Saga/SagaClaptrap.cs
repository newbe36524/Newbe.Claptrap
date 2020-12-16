using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Box;

namespace Newbe.Claptrap.Saga
{
    [ClaptrapEventHandler(typeof(SagaFlowCreateEventHandler), SagaCodes.Create)]
    [ClaptrapEventHandler(typeof(SagaCompensateStepEventHandler), SagaCodes.Compensate)]
    [ClaptrapEventHandler(typeof(SagaMoveToNextEventHandler), SagaCodes.MoveToNext)]
    public class SagaClaptrap : NormalClaptrapBox<ISagaStateData>, ISagaClaptrap
    {
        public delegate SagaClaptrap Factory(ISagaClaptrapIdentity identity);

        private readonly IClaptrapIdentity _identity;
        private readonly ISagaUserDataSerializer _sagaUserDataSerializer;
        private readonly ILogger<SagaClaptrap> _logger;
        private readonly Lazy<bool> _activated;

        public SagaClaptrap(IClaptrapIdentity identity,
            IClaptrapFactory claptrapFactory,
            IClaptrapAccessor claptrapAccessor,
            ISagaUserDataSerializer sagaUserDataSerializer,
            ILogger<SagaClaptrap> logger) : base(identity,
            claptrapFactory,
            claptrapAccessor)
        {
            _identity = identity;
            _sagaUserDataSerializer = sagaUserDataSerializer;
            _logger = logger;
            _activated = new Lazy<bool>(() =>
            {
                Claptrap.ActivateAsync().Wait();
                return true;
            });
        }

        public async Task RunAsync(SagaFlow flow)
        {
            _ = _activated.Value;
            if (StateData.SagaFlowState != null!)
            {
                if (StateData.SagaFlowState.IsCompleted)
                {
                    _logger.LogInformation("saga flow already completed");
                    return;
                }
            }
            else
            {
                _logger.LogInformation("saga flow is not built, start to build it");
                var evt = new SagaFlowCreateEvent
                {
                    Steps = flow.Steps,
                    CompensateSteps = flow.CompensateSteps,
                    UserData = _sagaUserDataSerializer.Serialize(flow.UserData, flow.UserData.GetType()),
                    UserDataType = flow.UserData.GetType()
                };

                await Claptrap.HandleEventAsync(new DataEvent(_identity, SagaCodes.Create, evt));
            }

            await ContinueAsync();
        }

        private async Task ContinueAsync()
        {
            _ = _activated.Value;
            var flowState = StateData.SagaFlowState;
            var maxSteps = flowState.Steps.Length + flowState.CompensateSteps.Length;
            var isNeedContinue = true;
            var indexNow = 0;
            while (isNeedContinue)
            {
                try
                {
                    isNeedContinue = await ContinueCoreAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "failed to continue saga flow");
                    throw;
                }

                indexNow++;
                if (indexNow >= maxSteps)
                {
                    _logger.LogInformation("run to max step count, force to break");
                    return;
                }
            }
        }

        private async Task<bool> ContinueCoreAsync()
        {
            var flowState = StateData.SagaFlowState;

            if (flowState.IsCompensated)
            {
                _logger.LogInformation("saga flow IsCompensated = true, please create a new saga flow.");
                return false;
            }

            if (flowState.IsCompleted)
            {
                _logger.LogInformation("saga flow IsCompleted = true, please create a new saga flow.");
                return false;
            }

            if (flowState.IsCompensating)
            {
                for (var i = 0;
                    i < flowState.CompensateStepStatuses.Length && i <= flowState.LastErrorStepIndex;
                    i++)
                {
                    var status = flowState.CompensateStepStatuses[i];
                    if (status != StepStatus.Completed)
                    {
                        await Claptrap.HandleEventAsync(new DataEvent(_identity,
                            SagaCodes.Compensate,
                            new SagaCompensateStepEvent
                            {
                                StepIndex = i
                            }));
                        return true;
                    }
                }
            }

            for (var i = 0; i < flowState.StepStatuses.Length; i++)
            {
                var status = flowState.StepStatuses[i];
                switch (status)
                {
                    case StepStatus.Completed:
                        continue;
                    case StepStatus.NotStarted:
                        await Claptrap.HandleEventAsync(new DataEvent(_identity,
                            SagaCodes.MoveToNext,
                            new SagaMoveToNextEvent
                            {
                                StepIndex = i
                            }));
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _logger.LogInformation("saga flow done");
            return false;
        }
    }
}