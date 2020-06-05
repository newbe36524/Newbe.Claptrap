using Newtonsoft.Json;

namespace Newbe.Claptrap.DesignStoreFormatter
{
    public class JsonClaptrapDesignStoreFormatter : IClaptrapDesignStoreFormatter
    {
        public string Format(IClaptrapDesignStore claptrapDesignStore)
        {
            var re = JsonConvert.SerializeObject(claptrapDesignStore, Formatting.Indented);
            return re;
        }
    }
}