using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.Core.Impl
{
    public class StateSavingFlow : IStateSavingFlow
    {
        private readonly StateSavingOptions _stateSavingOptions;
        private readonly IStateSaver _stateSaver;
        private readonly ILogger<StateSavingFlow> _logger;
        private readonly Subject<IState> _nextStateSeq;
        private IDisposable _snapshotSavingFlow = null!;

        public StateSavingFlow(
            StateSavingOptions stateSavingOptions,
            IStateSaver stateSaver,
            ILogger<StateSavingFlow> logger)
        {
            _stateSavingOptions = stateSavingOptions;
            _stateSaver = stateSaver;
            _logger = logger;
            _nextStateSeq = new Subject<IState>();
        }

        public void Activate()
        {
            var dist = _nextStateSeq
                .Where(state => state != null)
                .DistinctUntilChanged(state => state.Version);

            IObservable<IList<IState>> stateBuffer;
            var savingWindowTime = _stateSavingOptions.SavingWindowTime;
            var savingWindowVersionLimit = _stateSavingOptions.SavingWindowVersionLimit;
            if (savingWindowTime.HasValue && savingWindowVersionLimit.HasValue)
            {
                stateBuffer = dist.Buffer(savingWindowTime.Value,
                    savingWindowVersionLimit.Value);
            }
            else if (savingWindowTime.HasValue)
            {
                stateBuffer = dist.Buffer(savingWindowTime.Value);
            }
            else if (savingWindowVersionLimit.HasValue)
            {
                stateBuffer = dist.Buffer(savingWindowVersionLimit.Value);
            }
            else
            {
                _logger.LogInformation(
                    "there is no state saving window specified, state will not be save by every saving window.");
                stateBuffer = dist.Buffer(100)
                    .Where(x => false);
            }

            _snapshotSavingFlow = stateBuffer
                .Where(x => x.Count > 0)
                .Select(list =>
                {
                    var latestState = list.Last();
                    return Observable.FromAsync(async () =>
                    {
                        try
                        {
                            await SaveStateAsync(latestState);
                            _logger.LogInformation("state snapshot save, version : {version}",
                                latestState.Version);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(
                                e,
                                "thrown a exception when saving state snapshot, version : {version}",
                                latestState.Version);
                        }
                    });
                })
                .Concat()
                .Subscribe();
        }

        public void Deactivate()
        {
            _snapshotSavingFlow?.Dispose();
            _nextStateSeq?.Dispose();
        }

        public void OnNewStateCreated(IState state)
        {
            _nextStateSeq.OnNext(state);
        }

        public async Task SaveStateAsync(IState state)
        {
            _logger.LogDebug("start to save state");
            await _stateSaver.SaveAsync(state);
            _logger.LogInformation("state save success");
        }
    }
}