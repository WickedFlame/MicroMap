﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MicroMap.UnitTest.Datareader
{
    /// <summary>
    /// Gets IL setters and getters for a property
    /// </summary>
    internal static class DynamicProperties
    {
        #region Delegates

        public delegate object GenericGetter(object target);

        public delegate void GenericSetter(object target, object value);

        #endregion

        public static IList<Property> CreatePropertyMethods<T>()
        {
            var returnValue = new List<Property>();

            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                returnValue.Add(new Property(prop));
            }

            return returnValue;
        }

        public static IList<Property> CreatePropertyMethods(Type t)
        {
            var returnValue = new List<Property>();

            foreach (PropertyInfo prop in t.GetProperties())
            {
                returnValue.Add(new Property(prop));
            }

            return returnValue;
        }

        /// <summary>
        /// Creates a dynamic setter for the property
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static GenericSetter CreateSetMethod(PropertyInfo propertyInfo)
        {
            /*
            * If there's no setter return null
            */
            MethodInfo setMethod = propertyInfo.GetSetMethod();
            if (setMethod == null)
            {
                return null;
            }

            /*
            * Create the dynamic method
            */
            var arguments = new Type[2];
            arguments[0] = arguments[1] = typeof(object);

            var setter = new DynamicMethod(string.Concat("_Set", propertyInfo.Name, "_"), typeof(void), arguments, propertyInfo.DeclaringType);
            ILGenerator generator = setter.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
            generator.Emit(OpCodes.Ldarg_1);

            if (propertyInfo.PropertyType.IsClass)
            {
                generator.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
            }
            else
            {
                generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
            }

            generator.EmitCall(OpCodes.Callvirt, setMethod, null);
            generator.Emit(OpCodes.Ret);

            /*
            * Create the delegate and return it
            */
            return (GenericSetter)setter.CreateDelegate(typeof(GenericSetter));
        }


        /// <summary>
        /// Creates a dynamic getter for the property
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public static GenericGetter CreateGetMethod(PropertyInfo propertyInfo)
        {
            /*
            * If there's no getter return null
            */
            MethodInfo getMethod = propertyInfo.GetGetMethod();
            if (getMethod == null)
            {
                return null;
            }

            /*
            * Create the dynamic method
            */
            var arguments = new Type[1];
            arguments[0] = typeof(object);

            var getter = new DynamicMethod(string.Concat("_Get", propertyInfo.Name, "_"), typeof(object), arguments, propertyInfo.DeclaringType);
            ILGenerator generator = getter.GetILGenerator();
            generator.DeclareLocal(typeof(object));
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);
            generator.EmitCall(OpCodes.Callvirt, getMethod, null);

            if (!propertyInfo.PropertyType.IsClass)
            {
                generator.Emit(OpCodes.Box, propertyInfo.PropertyType);
            }

            generator.Emit(OpCodes.Ret);

            /*
            * Create the delegate and return it
            */
            return (GenericGetter)getter.CreateDelegate(typeof(GenericGetter));
        }

        #region Nested type: Property

        public class Property
        {
            public Property(PropertyInfo info)
            {
                Info = info;
                Setter = CreateSetMethod(info);
                Getter = CreateGetMethod(info);
            }

            public GenericGetter Getter { get; }

            public PropertyInfo Info { get; }

            public GenericSetter Setter { get; }
        }

        #endregion
    }
}
