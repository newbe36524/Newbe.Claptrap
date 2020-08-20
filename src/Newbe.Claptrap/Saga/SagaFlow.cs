using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.Saga
{
    public class SagaFlow
    {
        public Dictionary<string, string> UserData { get; set; }
        public IList<Type> Steps { get; set; }
        public IList<Type> CompensateSteps { get; set; }
    }
}