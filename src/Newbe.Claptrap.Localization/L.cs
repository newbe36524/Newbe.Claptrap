using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.Localization;

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap
{
    public class L : IL
    {
        private readonly IL _l;

        public L(
            ILogger<L> logger,
            IStringLocalizer<LK>? line)
        {
            if (line != null)
            {
                logger.LogInformation(
                    LK.There_is_a_IStringLocalizer_found_in_container__start_to_use_localization_in_UI_culture);
                _l = new StringLocalizerL(line);
            }
            else
            {
                logger.LogInformation(
                    LK.There_is_no_IStringLocalizer_found_in_container__start_to_use_default_localization_in_English);
                _l = new DefaultL();
            }
        }

        public string this[string index] => _l[index];

        public string this[string index, params object[] ps] => _l[index, ps];

        private class DefaultL : IL
        {
            public string this[string index, params object[] ps] =>
                string.Format(LK.ResourceManager.GetString(index), ps);

            public string this[string index] =>
                LK.ResourceManager.GetString(index);
        }

        private class StringLocalizerL : IL
        {
            private readonly IStringLocalizer<LK> _line;

            public StringLocalizerL(
                IStringLocalizer<LK> line)
            {
                _line = line;
            }

            public string this[string index] => _line[index];

            public string this[string index, params object[] ps] => _line[index, ps];
        }
    }
}