using System.Collections.Generic;
using System.Threading.Tasks;

namespace Newbe.Claptrap.DevTools.Translation
{
    public interface ITranslator
    {
        Task<TranslationResult[]> TranslateAsync(string sourceText, IEnumerable<string> targetCultures);
        Task<Dictionary<string, LocalizationFile>> TranslateAsync(LocalizationFile file, string[] targetCultures);
    }
}