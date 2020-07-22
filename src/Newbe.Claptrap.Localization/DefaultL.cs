namespace Newbe.Claptrap.Localization
{
    public class DefaultL : IL
    {
        public string this[string index, params object[] ps] =>
            string.Format(LK.ResourceManager.GetString(index), ps);

        public string this[string index] =>
            LK.ResourceManager.GetString(index);
    }
}