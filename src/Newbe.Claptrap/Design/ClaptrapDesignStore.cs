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

        private readonly IDictionary<string, ClaptrapDesignFactory> _factories
            = new Dictionary<string, ClaptrapDesignFactory>();

        public ClaptrapDesignStore(
            ILogger<ClaptrapDesignStore>? logger = null)
        {
            _logger = logger
                      ?? LoggerFactoryHolder.Instance?.CreateLogger<ClaptrapDesignStore>()
                      ?? NullLogger<ClaptrapDesignStore>.Instance;
        }

        public IClaptrapDesign FindDesign(IClaptrapIdentity claptrapIdentity)
        {
            var typeCode = claptrapIdentity.TypeCode;

            if (!_globalDic.TryGetValue(typeCode, out var globalDesign))
            {
                throw new ClaptrapDesignNotFoundException(claptrapIdentity);
            }

            if (_factories.TryGetValue(typeCode, out var factory))
            {
                var re = factory.Invoke(claptrapIdentity, globalDesign);
                return re;
            }

            return globalDesign;
        }

        public void AddOrReplace(IClaptrapDesign design)
        {
            var typeCode = design.ClaptrapTypeCode;
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

        public void AddOrReplaceFactory(string claptrapTypeCode, ClaptrapDesignFactory designFactory)
        {
            if (_factories.TryGetValue(claptrapTypeCode, out var old))
            {
                _logger.LogInformation(
                    "Found a old claptrap design factory and it will be replaced. typeCode : {@typeCode} old: {@designFactory}",
                    claptrapTypeCode,
                    old);
            }

            _factories[claptrapTypeCode] = designFactory;
            _logger.LogInformation(
                "a claptrap design factory add to factories. typeCode : {@typeCode} ",
                claptrapTypeCode);
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

        public void RemoveFactory(string claptrapTypeCode)
        {
            if (_factories.ContainsKey(claptrapTypeCode))
            {
                _factories.Remove(claptrapTypeCode);
                _logger.LogInformation("Remove a factory with claptrap type code : {claptrapTypeCode}",
                    claptrapTypeCode);
            }
            else
            {
                _logger.LogTrace("There is not factory found with claptrap type code : {claptrapTypeCode}",
                    claptrapTypeCode);
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