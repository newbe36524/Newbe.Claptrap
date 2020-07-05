using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newbe.Claptrap.Bootstrapper;

namespace Newbe.Claptrap.Design
{
    public class ClaptrapDesignStore : IClaptrapDesignStore
    {
        private readonly ILogger<ClaptrapDesignStore> _logger;

        public delegate ClaptrapDesignStore Factory();

        private readonly IDictionary<string, IClaptrapDesign> _globalDic
            = new Dictionary<string, IClaptrapDesign>();

        public ClaptrapDesignStore(
            ILogger<ClaptrapDesignStore>? logger = null)
        {
            _logger = logger
                      ?? LoggerFactoryHolder.Instance?.CreateLogger<ClaptrapDesignStore>()
                      ?? NullLogger<ClaptrapDesignStore>.Instance;
        }

        public IClaptrapDesign FindDesign(IClaptrapIdentity claptrapIdentity)
        {
            if (_globalDic.TryGetValue(claptrapIdentity.TypeCode, out var globalDesign))
            {
                return globalDesign;
            }

            throw new ClaptrapDesignNotFoundException(claptrapIdentity);
        }

        public void AddOrReplace(IClaptrapDesign design)
        {
            var typeCode = design.ClaptrapTypeCode;
            _logger.LogDebug("the claptrap design will be add to global store");
            if (_globalDic.TryGetValue(typeCode, out var old))
            {
                _logger.LogInformation(
                    "found a old claptrap design in global store and it will be replaced. old: {@design}",
                    old);
            }

            _globalDic[typeCode] = design;
            _logger.LogInformation(
                "a claptrap design add to global store. design: {@design}",
                design);
        }

        public void Remove(Func<IClaptrapDesign, bool> removedSelector)
        {
            var needRemoved = this.Where(removedSelector)
                .ToArray();
            foreach (var claptrapDesign in needRemoved)
            {
                var typeCode = claptrapDesign.ClaptrapTypeCode;
                if (_globalDic.Remove(typeCode))
                {
                    _logger.LogInformation("design for {typeCode} remove from global design store", typeCode);
                }
            }
        }

        public IEnumerator<IClaptrapDesign> GetEnumerator()
        {
            return _globalDic.Values.Cast<IClaptrapDesign>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}