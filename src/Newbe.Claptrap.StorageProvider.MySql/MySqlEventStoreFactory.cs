using System;

namespace Newbe.Claptrap.StorageProvider.MySql
{
    public class MySqlEventStoreFactory : IClaptrapComponentFactory<IEventLoader>,
        IClaptrapComponentFactory<IEventSaver>
    {
        IEventLoader IClaptrapComponentFactory<IEventLoader>.Create(IClaptrapIdentity claptrapIdentity)
        {
            throw new NotImplementedException();
        }

        IEventSaver IClaptrapComponentFactory<IEventSaver>.Create(IClaptrapIdentity claptrapIdentity)
        {
            throw new NotImplementedException();
        }
    }
}