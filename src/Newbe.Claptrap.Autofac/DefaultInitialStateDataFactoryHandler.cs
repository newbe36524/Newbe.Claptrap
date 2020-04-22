using System;
using System.Threading.Tasks;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.Metadata;

namespace Newbe.Claptrap.Autofac
{
    public class DefaultInitialStateDataFactoryHandler : IInitialStateDataFactoryHandler
    {
        private readonly IStateDataTypeRegister _stateDataTypeRegister;

        public DefaultInitialStateDataFactoryHandler(
            IStateDataTypeRegister stateDataTypeRegister)
        {
            _stateDataTypeRegister = stateDataTypeRegister;
        }

        public Task<IStateData> Create(IActorIdentity identity)
        {
            var findStateDataType = _stateDataTypeRegister.FindStateDataType(identity.TypeCode);
            var stateData = (IStateData) Activator.CreateInstance(findStateDataType);
            return Task.FromResult(stateData);
        }
    }
}