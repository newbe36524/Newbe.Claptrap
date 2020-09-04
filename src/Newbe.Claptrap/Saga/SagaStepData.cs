namespace Newbe.Claptrap.Saga
{
    public readonly struct SagaStepData
    {
        public SagaStepData(int stepIndex, SagaFlowState flowState, object userData, string flowId)
        {
            StepIndex = stepIndex;
            FlowState = flowState;
            UserData = userData;
            FlowId = flowId;
        }

        public int StepIndex { get; }
        public SagaFlowState FlowState { get; }
        public object UserData { get; }
        public string FlowId { get; }
    }

    public readonly struct SagaStepData<T>
    {
        public SagaStepData(int stepIndex, SagaFlowState flowState, T userData, string flowId)
        {
            StepIndex = stepIndex;
            FlowState = flowState;
            UserData = userData;
            FlowId = flowId;
        }

        public int StepIndex { get; }
        public SagaFlowState FlowState { get; }
        public T UserData { get; }
        public string FlowId { get; }
    }
}