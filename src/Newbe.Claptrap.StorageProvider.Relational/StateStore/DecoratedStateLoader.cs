using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public abstract class DecoratedStateLoader : IStateLoader
    {
        public IStateLoader StateLoader { get; }
        public IClaptrapIdentity Identity => StateLoader.Identity;

        protected DecoratedStateLoader(
            IStateLoader stateLoader)
        {
            StateLoader = stateLoader;
        }

        public abstract Task<IState?> GetStateSnapshotAsync();
    }
}