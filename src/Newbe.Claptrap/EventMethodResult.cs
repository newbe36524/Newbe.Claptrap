using Newbe.Claptrap.Core;

namespace Newbe.Claptrap
{
    public struct EventMethodResult<TEventData, TMethodReturn>
        where TEventData : class, IEventData
    {
        public bool EventRaising { get; set; }
        public TEventData EventData { get; set; }
        public IEventUid EventUid { get; set; }
        public TMethodReturn MethodReturn { get; set; }
    }

    public struct EventMethodResult<TEventData>
        where TEventData : class, IEventData
    {
        public bool EventRaising { get; set; }
        public TEventData EventData { get; set; }
        public IEventUid EventUid { get; set; }
    }

    public static class EventMethodResult
    {
        public static EventMethodResult<TEventData> Ok<TEventData>(TEventData eventData, IEventUid uid = null)
            where TEventData : class, IEventData
        {
            return new EventMethodResult<TEventData>
            {
                EventData = eventData,
                EventRaising = true,
                EventUid = uid
            };
        }

        public static EventMethodResult<TEventData> None<TEventData>()
            where TEventData : class, IEventData
        {
            return new EventMethodResult<TEventData>
            {
                EventRaising = true,
            };
        }

        public static EventMethodResult<TEventData, TMethodReturn> Ok<TEventData, TMethodReturn>(TEventData eventData,
            TMethodReturn methodReturn,
            IEventUid uid = null)
            where TEventData : class, IEventData
        {
            return new EventMethodResult<TEventData, TMethodReturn>
            {
                EventData = eventData,
                EventRaising = true,
                EventUid = uid,
                MethodReturn = methodReturn,
            };
        }

        public static EventMethodResult<TEventData, TMethodReturn> None<TEventData, TMethodReturn>(
            TMethodReturn returnValue)
            where TEventData : class, IEventData
        {
            return new EventMethodResult<TEventData, TMethodReturn>
            {
                MethodReturn = returnValue,
                EventRaising = true,
            };
        }
    }
}