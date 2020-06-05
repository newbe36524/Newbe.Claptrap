using Newbe.Claptrap.DesignStoreFormatter;

namespace Newbe.Claptrap.Bootstrapper
{
    public static class ClaptrapBootstrapperExtensions
    {
        public static string DumpDesignAsMarkdown(this IClaptrapBootstrapper bootstrapper,
            DesignStoreMarkdownFormatterOptions? options = null)
        {
            var claptrapDesignStore = bootstrapper.DumpDesignStore();
            var formatter = new DesignStoreMarkdownFormatter(options ?? new DesignStoreMarkdownFormatterOptions());
            var re = formatter.Format(claptrapDesignStore);
            return re;
        }

        public static string DumpDesignAsJson(this IClaptrapBootstrapper bootstrapper)
        {
            var claptrapDesignStore = bootstrapper.DumpDesignStore();
            var formatter = new JsonClaptrapDesignStoreFormatter();
            var re = formatter.Format(claptrapDesignStore);
            return re;
        }
    }
}