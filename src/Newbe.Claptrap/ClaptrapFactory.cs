using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.Extensions.Logging;
using static Newbe.Claptrap.LK.L0006ClaptrapFactory;

namespace Newbe.Claptrap
{
    public class ClaptrapFactory : IClaptrapFactory
    {
        private readonly ILogger<ClaptrapFactory> _logger;
        private readonly IClaptrapDesignStore _claptrapDesignStore;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IL _l;
        private readonly IEnumerable<IClaptrapModuleProvider> _claptrapModuleProviders;

        public ClaptrapFactory(
            ILogger<ClaptrapFactory> logger,
            IClaptrapDesignStore claptrapDesignStore,
            ILifetimeScope lifetimeScope,
            IL l,
            IEnumerable<IClaptrapModuleProvider> claptrapModuleProviders)
        {
            _logger = logger;
            _claptrapDesignStore = claptrapDesignStore;
            _lifetimeScope = lifetimeScope;
            _l = l;
            _claptrapModuleProviders = claptrapModuleProviders;
        }

        public IClaptrap Create(IClaptrapIdentity identity)
        {
            try
            {
                return CreateCore();
            }
            catch (Exception e)
            {
                _logger.LogError(e, _l[L001FailedToCreate], identity);
                throw;
            }

            IClaptrap CreateCore()
            {
                var claptrapDesign = _claptrapDesignStore.FindDesign(identity);
                var actorScope = _lifetimeScope.BeginLifetimeScope(builder =>
                {
                    var sharedModules = _claptrapModuleProviders
                        .SelectMany(x => x.GetClaptrapSharedModules(identity))
                        .OfType<Module>()
                        .ToArray();
                    _logger.LogTrace("Found {count} shared modules : {modules}",
                        sharedModules.Length,
                        sharedModules);
                    RegisterModules(sharedModules);

                    var masterDesign = claptrapDesign.ClaptrapMasterDesign;
                    if (masterDesign != null)
                    {
                        _logger.LogDebug(_l[L002MasterFound], masterDesign.ClaptrapTypeCode);
                        var minionModules = _claptrapModuleProviders
                            .SelectMany(x => x.GetClaptrapMinionModules(identity))
                            .OfType<Module>()
                            .ToArray();
                        _logger.LogTrace("Found {count} minion modules : {modules}",
                            minionModules.Length,
                            minionModules);
                        RegisterModules(minionModules);
                    }
                    else
                    {
                        _logger.LogDebug(_l[L003MasterFound], identity.TypeCode);
                        var masterModules = _claptrapModuleProviders
                            .SelectMany(x => x.GetClaptrapMasterClaptrapModules(identity))
                            .OfType<Module>()
                            .ToArray();
                        _logger.LogTrace("Found {count} master claptrap modules : {modules}",
                            masterModules.Length,
                            masterModules);
                        RegisterModules(masterModules);
                    }

                    void RegisterModules(IEnumerable<Module> modules)
                    {
                        foreach (var module in modules)
                        {
                            builder.RegisterModule(module);
                        }
                    }
                });
                var actor = actorScope.Resolve<IClaptrap>();
                return actor;
            }
        }
    }
}