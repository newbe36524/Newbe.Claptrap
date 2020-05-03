using Microsoft.Extensions.Localization;
using Newbe.Claptrap.Preview.Impl.Localization;

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap
{
    public class L : IL
    {
        private readonly IStringLocalizer _line;

        public L(
            IStringLocalizer line)
        {
            _line = line;
        }

        public string this[string index] => _line[index];

        public static IL Instance { get; internal set; } = null!;
    }
}