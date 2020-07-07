namespace Newbe.Claptrap
{
    /// <summary>
    /// Claptrap Accessor.
    /// You can change claptrap in the claptrap box by this accessor.
    /// It is used in unit test in common.
    /// </summary>
    public interface IClaptrapAccessor
    {
        IClaptrap? Claptrap { get; set; }
    }
}