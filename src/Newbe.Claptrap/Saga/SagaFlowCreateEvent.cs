using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.Saga
{
    public class SagaFlowCreateEvent : IEventData
    {
        public string UserData { get; set; } = null!;
        public Type UserDataType { get; set; } = null!;
        public IList<Type> Steps { get; set; } = null!;
        public IList<Type> CompensateSteps { get; set; } = null!;
    }
}