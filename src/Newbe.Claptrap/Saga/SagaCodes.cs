namespace Newbe.Claptrap.Saga
{
    public static class SagaCodes
    {
        public const string ClaptrapTypeCode = "saga_claptrap_newbe";
        private const string SagaEventSuffix = "_e_" + ClaptrapTypeCode;
        public const string Compensate = "compensate" + SagaEventSuffix;
        public const string MoveToNext = "moveToNext" + SagaEventSuffix;
        public const string Create = "create" + SagaEventSuffix;
    }
}