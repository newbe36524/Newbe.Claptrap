using System;

namespace Newbe.Claptrap.Preview.Abstractions.Metadata
{
    public interface IClaptrapDesignStoreAccessor
    {
        Type FindEventDataType(string actorTypeCode, string eventTypeCode);

        /// <summary>
        /// find state type
        /// </summary>
        /// <param name="actorTypeCode"></param>
        Type FindStateDataType(string actorTypeCode);

        /// <summary>
        /// find actor type code
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        string FindActorTypeCode(Type type);
    }
}