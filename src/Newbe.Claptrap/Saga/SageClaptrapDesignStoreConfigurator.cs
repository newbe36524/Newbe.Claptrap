using System.Collections.Generic;

namespace Newbe.Claptrap.Saga
{
    public class SageClaptrapDesignStoreConfigurator : IClaptrapDesignStoreConfigurator
    {
        public void Configure(IClaptrapDesignStore designStore)
        {
            designStore.AddOrReplaceFactory(SagaCodes.ClaptrapTypeCode, (identity, design) =>
            {
                var sagaClaptrapIdentity = (ISagaClaptrapIdentity) identity;
                var userDataType = sagaClaptrapIdentity.UserDataType;
                var stateDataType = typeof(SagaStateData<>);
                var finalType = stateDataType.MakeGenericType(userDataType);
                var newDesign = new ClaptrapDesign(design)
                {
                    StateDataType = finalType,
                };
                return newDesign;
            });
        }
    }
}