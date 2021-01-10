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

        private IState _state = null!;

        public IState State
        {
            get => _state;
            set
            {
                _logger.LogTrace("Update state, old state: {oldState} new state: {newState}",
                    _state,
                    value);
                _state = value;
            }
        }
    }
}