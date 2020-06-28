using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.Relational.Tools
{
    public class BatchOperatorKey : IBatchOperatorKey
    {
        private readonly IList<string> _parts = new List<string>();

        public BatchOperatorKey With(string part)
        {
            _parts.Add(part);
            return this;
        }

        public string AsStringKey()
        {
            return string.Join("-", _parts);
        }
    }
}