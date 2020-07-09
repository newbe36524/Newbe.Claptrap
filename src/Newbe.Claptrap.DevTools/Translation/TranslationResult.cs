namespace Newbe.Claptrap.DevTools.Translation
{
    /// <summary>
    /// The C# classes that represents the JSON returned by the Translator.
    /// </summary>
    public class TranslationResult
    {
        public DetectedLanguage DetectedLanguage { get; set; }
        public TextResult SourceText { get; set; }
        public Translation[] Translations { get; set; }
    }
}