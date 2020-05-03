using System.Collections.Generic;

namespace Newbe.Claptrap.DevTools
{
    public class LocalizationFile
    {
        public IEnumerable<LocalizationItem> Items { get; set; }
        public string Culture { get; set; }
    }
}