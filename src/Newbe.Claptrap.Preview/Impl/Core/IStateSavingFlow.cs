using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
{
    public interface IStateSavingFlow
    {
        void Activate();
        void Deactivate();
        void OnNewStateCreated(IState state);
        Task SaveStateAsync(IState state);
    }
}