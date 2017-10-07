using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MicroMap.TypeDefinition
{
    public static class TypeDefinitionFactory
    {
        /// <summary>
        /// Gets all fielddefinitions that can be created from the type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<FieldDefinition> GetFieldDefinitions<T>()
        {
            return ExtractFieldDefinitions(typeof(T));
        }

        ///// <summary>
        ///// Gets a list of fields that are common to two types. This is used when a concrete and anonymous type definition have to be matched
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //public static IEnumerable<FieldDefinition> GetFieldDefinitions<T>(Type type)
        //{
        //    var definedFields = ExtractFieldDefinitions(typeof(T));
        //    var objectDefinitions = ExtractFieldDefinitions(type).ToList();

        //    foreach (var field in objectDefinitions)
        //    {
        //        // merge the fields from the defined type to the provided type (anonymous object)
        //        var defined = definedFields.FirstOrDefault(f => f.MemberName == field.MemberName);
        //        if (defined == null)
        //        {
        //            continue;
        //        }

        //        field.IsNullable = defined.IsNullable;
        //        field.IsPrimaryKey = defined.IsPrimaryKey;
        //        field.EntityName = defined.EntityName;
        //        field.EntityType = defined.EntityType;
        //        field.PropertyInfo = defined.PropertyInfo;
        //        field.FieldType = defined.FieldType;
        //    }

        //    return objectDefinitions;
        //}

        /// <summary>
        /// Gets all fielddefinitions that can be created by the type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<FieldDefinition> GetFieldDefinitions(this Type type)
        {
            return ExtractFieldDefinitions(type);
        }

        #region Internal Implementation

        static Dictionary<Type, IEnumerable<FieldDefinition>> _fieldDefinitionCache;

        /// <summary>
        /// Cach dictionary that containes all fielddefinitions belonging to a given type
        /// </summary>
        private static Dictionary<Type, IEnumerable<FieldDefinition>> FieldDefinitionCache
        {
            get
            {
                if (_fieldDefinitionCache == null)
                {
                    _fieldDefinitionCache = new Dictionary<Type, IEnumerable<FieldDefinition>>();
                }

                return _fieldDefinitionCache;
            }
        }

        private static IEnumerable<FieldDefinition> ExtractFieldDefinitions(Type type)
        {
            IEnumerable<FieldDefinition> fields = new List<FieldDefinition>();
            if (!FieldDefinitionCache.TryGetValue(type, out fields))
            {
                fields = type.GetSelectionMembers().Select(m => m.ToFieldDefinition());
                FieldDefinitionCache.Add(type, fields);
            }

            return fields;
        }

        private static FieldDefinition ToFieldDefinition(this PropertyInfo propertyInfo)
        {
            var isNullableType = propertyInfo.PropertyType.IsNullableType();

            var isNullable = !propertyInfo.PropertyType.IsValueType || isNullableType;

            var propertyType = isNullableType ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;

            return new FieldDefinition
            {
                FieldName = propertyInfo.Name,
                MemberName = propertyInfo.Name,
                EntityName = propertyInfo.DeclaringType.Name,
                MemberType = propertyType,
                FieldType = propertyType,
                EntityType = propertyInfo.DeclaringType,
                IsNullable = isNullable,
                PropertyInfo = propertyInfo,
                IsPrimaryKey = CheckPrimaryKey(propertyInfo, propertyInfo.DeclaringType.Name),
                //GetValueFunction = propertyInfo.GetPropertyGetter(),
                SetValueFunction = propertyInfo.GetPropertySetter(),
            };
        }

        private static bool CheckPrimaryKey(PropertyInfo propertyInfo, string memberName)
        {
            // extremely simple convention that says the key element has to be called ID or {Member}ID
            return propertyInfo.Name.ToLower().Equals("id") ||
                   propertyInfo.Name.ToLower().Equals(string.Format("{0}id", memberName.ToLower()));
        }

        #endregion
    }
}
