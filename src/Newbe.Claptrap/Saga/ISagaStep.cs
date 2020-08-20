using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.Saga
{
    public interface ISagaStep
    {
        Task RunAsync(int stepIndex, SagaFlowState flowState, Dictionary<string, string> userData);
    }
}