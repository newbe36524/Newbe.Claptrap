using System;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public class MySqlStateStoreFactory : IClaptrapComponentFactory<IStateLoader>,
        IClaptrapComponentFactory<IStateSaver>
    {
        IStateLoader IClaptrapComponentFactory<IStateLoader>.Create(IClaptrapIdentity claptrapIdentity)
        {
            throw new NotImplementedException();
        }

        IStateSaver IClaptrapComponentFactory<IStateSaver>.Create(IClaptrapIdentity claptrapIdentity)
        {
            throw new NotImplementedException();
        }
    }
}