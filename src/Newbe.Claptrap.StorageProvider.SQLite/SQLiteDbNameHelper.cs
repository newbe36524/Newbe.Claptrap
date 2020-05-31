using System.IO;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public static class SQLiteConnectionNameHelper
    {
        public static string OneIdOneFileEventStore(
            IClaptrapIdentity masterOrSelfIdentity)
        {
            return Path.Combine($"{masterOrSelfIdentity.TypeCode}_{masterOrSelfIdentity.Id}", "eventDb.db");
        }

        public static string OneIdOneFileStateStore(
            IClaptrapDesign design,
            IClaptrapIdentity identity)
        {
            var typeCode = design.ClaptrapTypeCode;
            return Path.Combine($"{typeCode}_{identity.Id}", "stateDb.db");
        }
    }
}