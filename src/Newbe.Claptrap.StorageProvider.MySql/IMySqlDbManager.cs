using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public interface IMySqlDbManager
    {
        void CreateOrUpdateDatabase(IClaptrapIdentity identity,
            Func<string, bool> sqlSelector,
            Dictionary<string,string> variables);
    }
}