namespace Newbe.Claptrap.Saga
{
    public class SagaMoveToNextEvent : IEventData
    {
        public int StepIndex { get; set; }
    }
}