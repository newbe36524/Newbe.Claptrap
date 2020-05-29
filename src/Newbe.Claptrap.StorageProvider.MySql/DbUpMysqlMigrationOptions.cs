using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public class DbUpMysqlMigrationOptions
    {
        public string DbName { get; set; }
        public Func<string, bool> SqlSelector { get; set; }
        public Dictionary<string, string> Variables { get; set; }
    }
}