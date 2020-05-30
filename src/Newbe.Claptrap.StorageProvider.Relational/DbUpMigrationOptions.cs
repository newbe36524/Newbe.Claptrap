using System;
using System.Collections.Generic;
using System.Reflection;
using DbUp.Builder;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public class DbUpMigrationOptions
    {
        public DbUpMigrationOptions(IEnumerable<Assembly> scriptAssemblies,
            Func<string, bool> scriptSelector,
            Dictionary<string, string> variables,
            Func<UpgradeEngineBuilder> upgradeEngineBuilderFactory,
            bool enableNullJournal)
        {
            ScriptAssemblies = scriptAssemblies;
            ScriptSelector = scriptSelector;
            Variables = variables;
            UpgradeEngineBuilderFactory = upgradeEngineBuilderFactory;
            EnableNullJournal = enableNullJournal;
        }

        public IEnumerable<Assembly> ScriptAssemblies { get; }
        public Func<string, bool> ScriptSelector { get; }
        public Dictionary<string, string> Variables { get; }
        public Func<UpgradeEngineBuilder> UpgradeEngineBuilderFactory { get; }
        public bool EnableNullJournal { get; }
    }
}