using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newbe.Claptrap.Preview.Abstractions.Core;
using Newbe.Claptrap.Preview.Abstractions.Exceptions;
using Newbe.Claptrap.Preview.Abstractions.Metadata;
using Newbe.Claptrap.Preview.Logging;

namespace Newbe.Claptrap.Preview.Impl
{
    public class ClaptrapDesignStore : IClaptrapDesignStore
    {
        public delegate ClaptrapDesignStore Factory();

        private static readonly ILog Logger = LogProvider.For<ClaptrapDesignStore>(); 

        private readonly IDictionary<string, IClaptrapDesign> _globalDic
            = new Dictionary<string, IClaptrapDesign>();

        private readonly IDictionary<StructClaptrapIdentity, IClaptrapDesign> _idDic =
            new Dictionary<StructClaptrapIdentity, IClaptrapDesign>();


        public IClaptrapDesign FindDesign(IClaptrapIdentity claptrapIdentity)
        {
            if (!string.IsNullOrEmpty(claptrapIdentity.Id))
            {
                if (_idDic.TryGetValue(new StructClaptrapIdentity(claptrapIdentity), out var idSpecifiedDesign))
                {
                    return idSpecifiedDesign;
                }
            }

            if (_globalDic.TryGetValue(claptrapIdentity.TypeCode, out var globalDesign))
            {
                return globalDesign;
            }

            throw new ClaptrapDesignNotFoundException(claptrapIdentity);
        }

        public void AddOrReplace(IClaptrapDesign design)
        {
            var typeCode = design.Identity.TypeCode;
            var id = design.Identity.Id;
            Logger.Debug("start to add or replace a claptrap design [{typeCode} : {id}]",
                typeCode,
                id);
            if (string.IsNullOrEmpty(id))
            {
                Logger.Debug("id is null and the claptrap design will be add to global store");
                if (_globalDic.TryGetValue(typeCode, out var old))
                {
                    Logger.Info(
                        "found a old claptrap design in global store and it will be replaced. old: {@design}",
                        old);
                }

                _globalDic[typeCode] = design;
                Logger.Info(
                    "a claptrap design add to global store. design: {@design}",
                    design);
            }
            else
            {
                Logger.Debug("id is {id} and the claptrap design will be add to id specified store", id);
                var key = new StructClaptrapIdentity(design.Identity);
                if (_idDic.TryGetValue(key, out var old))
                {
                    Logger.Info(
                        "found a old claptrap design in id specified store and it will be replaced. old: {@design}",
                        old);
                }

                _idDic[key] = design;
                Logger.Info(
                    "a claptrap design add to id specified store. design: {@design}",
                    design);
            }
        }

        public void Remove(Func<IClaptrapDesign, bool> removedSelector)
        {
            var needRemoved = this.Where(removedSelector)
                .ToArray();
            foreach (var claptrapDesign in needRemoved)
            {
                var identity = claptrapDesign.Identity;
                if (_globalDic.Remove(identity.TypeCode))
                {
                    Logger.Info("design for {identity} remove from global design store", identity);
                }

                if (_idDic.Remove(new StructClaptrapIdentity(identity)))
                {
                    Logger.Info("design for {identity} remove from id specified store", identity);
                }
            }
        }

        private readonly struct StructClaptrapIdentity : IClaptrapIdentity
        {
            public StructClaptrapIdentity(IClaptrapIdentity identity)
            {
                Id = identity.Id;
                TypeCode = identity.TypeCode;
            }

            public bool Equals(IClaptrapIdentity other)
            {
                return Id == other.Id && TypeCode == other.TypeCode;
            }

            public override bool Equals(object? obj)
            {
                return obj is StructClaptrapIdentity other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Id, TypeCode);
            }

            public string Id { get; }
            public string TypeCode { get; }
        }

        public IEnumerator<IClaptrapDesign> GetEnumerator()
        {
            foreach (var (_, value) in _globalDic)
            {
                yield return value;
            }

            foreach (var (_, value) in _idDic)
            {
                yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}