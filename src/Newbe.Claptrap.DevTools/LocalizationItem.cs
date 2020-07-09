namespace Newbe.Claptrap.DevTools
{
    public class LocalizationItem
    {
        public string Text { get; set; }
        public string Key { get; set; }
        public string SourceText { get; set; }

        public override string ToString()
        {
            return $"{Key} = {Text} # {SourceText}";
        }
    }
}