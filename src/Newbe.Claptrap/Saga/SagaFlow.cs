using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.Saga
{
    public class SagaFlow
    {
        public object UserData { get; set; }
        public IList<Type> Steps { get; set; }
        public IList<Type> CompensateSteps { get; set; }
    }
}