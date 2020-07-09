using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using DbUp.Builder;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public class DbUpMigrationOptions
    {
        public DbUpMigrationOptions(IEnumerable<Assembly> scriptAssemblies,
            Func<string, bool> scriptSelector,
            Dictionary<string, string> variables,
            Func<(UpgradeEngineBuilder builder, IDbConnection? dbConnection)> upgradeEngineBuilderFactory)
        {
            ScriptAssemblies = scriptAssemblies;
            ScriptSelector = scriptSelector;
            Variables = variables;
            UpgradeEngineBuilderFactory = upgradeEngineBuilderFactory;
        }

        public IEnumerable<Assembly> ScriptAssemblies { get; }
        public Func<string, bool> ScriptSelector { get; }
        public Dictionary<string, string> Variables { get; }
        public Func<(UpgradeEngineBuilder builder, IDbConnection? dbConnection)> UpgradeEngineBuilderFactory { get; }
    }
}