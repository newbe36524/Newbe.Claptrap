using System.Threading.Tasks;

namespace Newbe.Claptrap.Saga
{
    [ClaptrapState(typeof(SagaStateData), SagaCodes.ClaptrapTypeCode)]
    [ClaptrapEvent(typeof(SagaFlowCreateEvent), SagaCodes.Create)]
    [ClaptrapEvent(typeof(SagaCompensateStepEvent), SagaCodes.Compensate)]
    [ClaptrapEvent(typeof(SagaMoveToNextEvent), SagaCodes.MoveToNext)]
    public interface ISagaClaptrap
    {
        Task RunAsync(SagaFlow flow);
        Task ContinueAsync();
    }
}