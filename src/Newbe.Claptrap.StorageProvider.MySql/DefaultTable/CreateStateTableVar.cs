using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.MySql.DefaultTable
{
    public class CreateStateTableVar
    {
        public string SchemaName { get; set; }
        public string StateTableName { get; set; }

        public Dictionary<string, string> GetDictionary()
        {
            return new Dictionary<string, string>
            {
                {nameof(SchemaName), SchemaName},
                {nameof(StateTableName), StateTableName},
            };
        }
    }
}