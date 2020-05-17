using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.MySql.DefaultTable
{
    public class CreateEventTableVars
    {
        public string SchemaName { get; set; }
        public string EventTableName { get; set; }

        public Dictionary<string, string> GetDictionary()
        {
            return new Dictionary<string, string>
            {
                {nameof(SchemaName), SchemaName},
                {nameof(EventTableName), EventTableName}
            };
        }
    }
}