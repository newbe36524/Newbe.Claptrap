namespace Newbe.Claptrap.Saga
{
    public class SagaCompensateStepEvent : IEventData
    {
        public int StepIndex { get; set; }
    }
}