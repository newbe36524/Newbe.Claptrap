using System.Threading.Tasks;

namespace Newbe.Claptrap.Core
{
    public interface IStateSavingFlow
    {
        void Activate();
        void Deactivate();
        void OnNewStateCreated(IState state);
        Task SaveStateAsync(IState state);
    }
}