namespace Newbe.Claptrap.Saga
{
    public interface ISagaStateData : IStateData
    {
        SagaFlowState SagaFlowState { get; set; }
        object GetUserData();
        void SetUserData(object userData);
    }
}