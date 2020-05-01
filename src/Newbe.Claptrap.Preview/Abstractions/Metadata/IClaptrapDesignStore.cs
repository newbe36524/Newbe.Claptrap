using System;
using System.Collections.Generic;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Exceptions;

namespace Newbe.Claptrap.Preview.Abstractions.Metadata
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
        /// remove design where matched selector
        /// </summary>
        /// <param name="removedSelector"></param>
        void Remove(Func<IClaptrapDesign, bool> removedSelector);
    }
}