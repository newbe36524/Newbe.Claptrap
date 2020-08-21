using System;

namespace Newbe.Claptrap.Saga
{
    public interface ISagaUserDataSerializer
    {
        string Serialize(object userData, Type userDataType);
        object Deserialize(string source, Type userDataType);
    }
}