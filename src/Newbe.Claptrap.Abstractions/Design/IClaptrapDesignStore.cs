using System;
using System.Collections.Generic;

namespace Newbe.Claptrap
{
    public interface IClaptrapDesignStore : IEnumerable<IClaptrapDesign>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="claptrapIdentity"></param>
        /// <exception cref="ClaptrapDesignNotFoundException">thrown if no match design found</exception>
        /// <returns></returns>
        IClaptrapDesign FindDesign(IClaptrapIdentity claptrapIdentity);

        void AddOrReplace(IClaptrapDesign design);

        /// <summary>
        /// Add or replace a design factory
        /// </summary>
        /// <param name="claptrapTypeCode"></param>
        /// <param name="designFactory"></param>
        void AddOrReplaceFactory(string claptrapTypeCode, ClaptrapDesignFactory designFactory);

        /// <summary>
        /// remove design where matched selector
        /// </summary>
        /// <param name="removedSelector"></param>
        void Remove(Func<IClaptrapDesign, bool> removedSelector);

        void RemoveFactory(string claptrapTypeCode);
    }

    /// <summary>
    /// Create a claptrap design from sourceDesign and Identity
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="sourceDesign"></param>
    public delegate IClaptrapDesign ClaptrapDesignFactory(IClaptrapIdentity identity, IClaptrapDesign sourceDesign);
}