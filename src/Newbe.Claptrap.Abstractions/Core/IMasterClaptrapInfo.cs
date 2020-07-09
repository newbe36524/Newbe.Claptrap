namespace Newbe.Claptrap
{
    /// <summary>
    /// Info about the master of minion
    /// </summary>
    public interface IMasterClaptrapInfo
    {
        IClaptrapIdentity Identity { get; }
        IClaptrapDesign Design { get; }
    }
}