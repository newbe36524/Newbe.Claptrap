namespace Newbe.Claptrap
{
    public interface IL
    {
        string this[string index, params object[] ps] { get; }
        string this[string index] { get; }
    }
}