namespace Newbe.Claptrap.Core
{
    public interface IClaptrapKind : IActorKind
    {
        /// <summary>
        /// catalog of actor. that can be related to business info.
        /// </summary>
        string Catalog { get; }
    }
}