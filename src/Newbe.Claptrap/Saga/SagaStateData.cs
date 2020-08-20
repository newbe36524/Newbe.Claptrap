using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.Saga
{
    public class SagaStateData : ISagaStateData
    {
        public SagaFlowState SagaFlowState { get; set; } = null!;
        public Dictionary<string, string> UserData { get; set; } = null!;
    }

    public class SagaFlowState
    {
        public Type[] Steps { get; set; } = null!;
        public StepStatus[] StepStatuses { get; set; } = null!;
        public int LastErrorStepIndex { get; set; }
        public Type[] CompensateSteps { get; set; } = null!;
        public StepStatus[] CompensateStepStatuses { get; set; } = null!;
        public bool IsCompleted { get; set; }
        public bool IsCompensating { get; set; }
        public bool IsCompensated { get; set; }
    }

    public interface ISagaStateData : IStateData
    {
        SagaFlowState SagaFlowState { get; set; }
        Dictionary<string, string> UserData { get; set; }
    }
}