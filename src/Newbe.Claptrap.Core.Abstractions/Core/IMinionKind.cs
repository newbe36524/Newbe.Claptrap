namespace Newbe.Claptrap.Core
{
    public interface IMinionKind : IActorKind
    {
        /// <summary>
        /// catalog of actor. that can be related to business info.
        /// this should be the same as Claptrap
        /// </summary>
        string Catalog { get; }

        string MinionCatalog { get; }
    }
}