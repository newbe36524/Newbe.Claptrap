using System;

namespace Newbe.Claptrap.Saga
{
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
}