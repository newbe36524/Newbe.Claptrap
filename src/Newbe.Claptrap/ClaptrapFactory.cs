using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using MethodTimer;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Localization;

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

        [Time]
        public IClaptrap Create(IClaptrapIdentity identity)
        {
            try
            {
                var actorScope = BuildClaptrapLifetimeScope(identity);
                var actor = actorScope.Resolve<IClaptrap>();
                return actor;
            }
            catch (Exception e)
            {
                _logger.LogError(e, _l[LK.failed_to_create_a_claptrap___identity_], identity);
                throw;
            }
        }

        public ILifetimeScope BuildClaptrapLifetimeScope(IClaptrapIdentity identity)
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
                    _logger.LogTrace(_l[LK.IsMinion], masterDesign.ClaptrapTypeCode);
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
                    _logger.LogTrace(_l[LK.This_is_a_master_claptrap__type_code____typeCode_], identity.TypeCode);
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
            return actorScope;
        }
    }
}