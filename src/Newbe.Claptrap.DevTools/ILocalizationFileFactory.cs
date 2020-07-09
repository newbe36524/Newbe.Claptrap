namespace Newbe.Claptrap.DevTools
{
    public interface ILocalizationFileFactory
    {
        string Create(LocalizationFile file);
        LocalizationFile ResolveFormContent(string content);
    }
}