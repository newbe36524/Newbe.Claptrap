namespace Newbe.Claptrap.Saga
{
    public class SagaStateData<T> : ISagaStateData
        where T : class
    {
        public SagaFlowState SagaFlowState { get; set; } = null!;
        public T UserData { get; set; } = null!;

        public object GetUserData()
        {
            return UserData;
        }

        public void SetUserData(object userData)
        {
            UserData = (T) userData;
        }
    }
}