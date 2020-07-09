using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newbe.Claptrap.Localization;

// ReSharper disable once CheckNamespace
namespace Newbe.Claptrap
{
    // ReSharper disable once InconsistentNaming
    public static partial class LK
    {
        /// <summary>
        /// init class properties value in nest LK classes
        /// </summary>
        /// <exception cref="LKTypeDefinitionErrorException">thrown if some validation failed</exception>
        public static void Init()
        {
            var lkClass = typeof(LK);
            var nestedTypes = lkClass.GetNestedTypes();

            var errors = Check(nestedTypes).Where(x => !string.IsNullOrEmpty(x));
            var error = string.Join(",", errors);
            if (!string.IsNullOrEmpty(error))
            {
                throw new LKTypeDefinitionErrorException($"there are some error in LK class : {error}");
            }

            foreach (var nestedType in nestedTypes)
            {
                InitOne(nestedType);
            }

            static void InitOne(Type type)
            {
                var prefix = $"{nameof(LK)}.{type.Name.Substring(0, "LXXXX".Length)}.";
                var propertyInfos = type.GetProperties();
                foreach (var propertyInfo in propertyInfos)
                {
                    const string subPrefix = "XXX";
                    // e.g. L001BuildException -> Prefix + 001
                    var value = prefix + propertyInfo.Name.Substring(1, subPrefix.Length);
                    propertyInfo.SetValue(null, value);
                }
            }

            IEnumerable<string> Check(IEnumerable<Type> types)
            {
                const string classNamePrefix = "L####";
                const string propertyNamePrefixFormat = "L###";
                foreach (var type in types)
                {
                    yield return CheckType(type);
                    foreach (var propertyInfo in type.GetProperties())
                    {
                        yield return CheckProperty(propertyInfo);
                    }

                    var propertyLookup = type.GetProperties()
                        .Where(x => x.Name.Length > propertyNamePrefixFormat.Length)
                        .ToLookup(x => x.Name.Substring(0, propertyNamePrefixFormat.Length));
                    foreach (var group in propertyLookup.Where(x => x.Count() > 1))
                    {
                        var names = string.Join(",", group.Select(x => x.Name));
                        yield return
                            $"properties {names} have the same property name prefix {group.Key}, please make them different";
                    }
                }

                var classNames = types
                    .Where(x => x.Name.Length > classNamePrefix.Length)
                    .ToLookup(x => x.Name.Substring(0, classNamePrefix.Length));

                foreach (var group in classNames.Where(x => x.Count() > 1))
                {
                    var names = string.Join(",", group.Select(x => x.Name));
                    yield return $"classes {names} have the same class prefix {group.Key}, please make them different";
                }

                string CheckType(Type type)
                {
                    if (type.Name.Length < classNamePrefix.Length)
                    {
                        return
                            $"type name {type.Name} is too short, please confirm that is in {classNamePrefix}XXX format";
                    }

                    var classPrefix = type.Name.Substring(0, classNamePrefix.Length);
                    if (!classPrefix.StartsWith("L"))
                    {
                        return
                            $"type name {type.Name} is not start with 'L', please confirm that is in {classNamePrefix}XXX format";
                    }

                    var numberString = classPrefix.TrimStart('L');
                    if (!int.TryParse(numberString, out _))
                    {
                        return
                            $"type name {type.Name} is not a number after 'L', please confirm that is in {classNamePrefix}XXX format";
                    }

                    return string.Empty;
                }

                string CheckProperty(PropertyInfo info)
                {
                    var infoName = info.Name;
                    var prefix = infoName.Substring(0, propertyNamePrefixFormat.Length);
                    if (!prefix.StartsWith("L"))
                    {
                        return
                            $"property name {info.Name} is not start with 'L', please confirm that is in {propertyNamePrefixFormat}XXX format";
                    }

                    var numberString = prefix.TrimStart('L');
                    if (!int.TryParse(numberString, out _))
                    {
                        return
                            $"property name {info.Name} is not a number after 'L', please confirm that is in {propertyNamePrefixFormat}XXX format";
                    }

                    if (!info.CanRead)
                    {
                        return $"property {info.Name} is no readable, please add getter to it";
                    }

                    if (!info.CanWrite)
                    {
                        return $"property {info.Name} is no writable, please add setter to it";
                    }

                    try
                    {
                        _ = info.GetValue(null);
                    }
                    catch (Exception)
                    {
                        return $"property {info.Name} is not static, please make it a static property";
                    }

                    return string.Empty;
                }
            }
        }
    }
}