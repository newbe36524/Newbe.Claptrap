namespace Newbe.Claptrap.Orleans
{
    public static class ClaptrapGrainExtensions
    {
        public static DataEvent CreateEvent<TStateData, TEventDataType>(
            this IClaptrapBoxGrain<TStateData> claptrapBox,
            TEventDataType eventData)
            where TStateData : IStateData
            where TEventDataType : IEventData
        {
            var eventTypeCode =
                claptrapBox.ClaptrapGrainCommonService.ClaptrapTypeCodeFactory.FindEventTypeCode(claptrapBox, eventData);
            var dataEvent = new DataEvent(claptrapBox.Claptrap.State.Identity,
                eventTypeCode,
                eventData);
            return dataEvent;
        }
    }
}