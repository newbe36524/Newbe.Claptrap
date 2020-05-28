using System;

namespace Newbe.Claptrap.Bootstrapper
{
    public interface ISQLiteProviderEvenStoreConfigurator
    {
        ISQLiteProviderEvenStoreConfigurator ConfigureOptions(Action<ClaptrapStorageProviderOptions> optionsAction);
    }
}