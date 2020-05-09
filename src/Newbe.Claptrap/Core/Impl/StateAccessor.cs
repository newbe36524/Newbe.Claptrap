using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Core.Impl
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