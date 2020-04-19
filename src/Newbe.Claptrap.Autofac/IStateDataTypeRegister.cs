using System;

namespace Newbe.Claptrap.Autofac
{
    public interface IStateDataTypeRegister
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actorTypeCode"></param>
        Type FindStateDataType(string actorTypeCode);

        void RegisterStateDataType(string actorTypeCode, Type stateDataType);
    }
}