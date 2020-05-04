using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
{
    public class StateAccessor : IStateAccessor
    {
        private readonly ILogger<StateAccessor> _logger;

        public StateAccessor(
            ILogger<StateAccessor> logger)
        {
            _logger = logger;
        }

        private IState _state;

        public IState State
        {
            get { return _state; }
            set { _state = value; }
        }
    }
}