using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.Threading;

namespace MicroMap.Mapper
{
    //public delegate EmptyConstructorDelegate EmptyConstructorFactoryDelegate(Type type);

    public delegate object EmptyConstructorDelegate();

    /// <summary>
    /// Factory Class that generates instances of a type
    /// </summary>
    internal static class InstanceFactory
    {
        static Dictionary<Type, EmptyConstructorDelegate> _constructorMethods = new Dictionary<Type, EmptyConstructorDelegate>();

        /// <summary>
        /// Factory Method that creates an instance of type T
        /// </summary>
        /// <typeparam name="T">The type to create an instance of</typeparam>
        /// <returns>An instance of type T</returns>
        public static T CreateInstance<T>()
        {
            return (T)ConstructorProvider<T>.EmptyConstructorFunction();
        }
        
        /// <summary>
        /// Anonymous objects have a constructor that accepts all arguments in the same order as defined
        /// To populate a anonymous object the data has to be passed in the same order as defined to the constructor
        /// </summary>
        /// <typeparam name="T">The object type to crate</typeparam>
        /// <param name="args">The list of arguments needed for constructing the anonmous object</param>
        /// <returns>An instance of an anonymous object T</returns>
        public static T CreateAnonymousObject<T>(IEnumerable<object> args)
        {
            return (T)Activator.CreateInstance(typeof(T), args.ToArray());
        }
        
        private static EmptyConstructorDelegate GetConstructorMethodToCache(Type type)
        {
            if (type.IsInterface)
            {
                if (type.HasGenericType())
                {
                    var genericType = type.GetTypeWithGenericTypeDefinitionOfAny(typeof(IDictionary<,>));

                    if (genericType != null)
                    {
                        var keyType = genericType.GetGenericArguments()[0];
                        var valueType = genericType.GetGenericArguments()[1];
                        return GetConstructorMethodToCache(typeof(Dictionary<,>).MakeGenericType(keyType, valueType));
                    }

                    genericType = type.GetTypeWithGenericTypeDefinitionOfAny(typeof(IEnumerable<>), typeof(ICollection<>), typeof(IList<>));

                    if (genericType != null)
                    {
                        var elementType = genericType.GetGenericArguments()[0];
                        return GetConstructorMethodToCache(typeof(List<>).MakeGenericType(elementType));
                    }
                }
            }
            else if (type.IsArray)
            {
                return () => Array.CreateInstance(type.GetElementType(), 0);
            }
            else if (type.IsGenericTypeDefinition)
            {
                var genericArgs = type.GetGenericArguments();
                var typeArgs = new Type[genericArgs.Length];
                for (var i = 0; i < genericArgs.Length; i++)
                {
                    typeArgs[i] = typeof(object);
                }

                var realizedType = type.MakeGenericType(typeArgs);
                return realizedType.CreateInstance;
            }

            var emptyCtor = type.GetEmptyConstructor();
            if (emptyCtor != null)
            {
                var dynamicMethod = new DynamicMethod("MyCtor", type, Type.EmptyTypes, type.Module, true);

                var ilGenerator = dynamicMethod.GetILGenerator();
                ilGenerator.Emit(System.Reflection.Emit.OpCodes.Nop);
                ilGenerator.Emit(System.Reflection.Emit.OpCodes.Newobj, emptyCtor);
                ilGenerator.Emit(System.Reflection.Emit.OpCodes.Ret);

                return (EmptyConstructorDelegate)dynamicMethod.CreateDelegate(typeof(EmptyConstructorDelegate));
            }

            if (type == typeof(string))
            {
                return () => string.Empty;
            }

            // Anonymous types don't have empty constructors
            return () => FormatterServices.GetUninitializedObject(type);
        }

        private static object CreateInstance(this Type type)
        {
            if (type == null)
            {
                return null;
            }

            return GetConstructorMethod(type).Invoke();
        }

        private static EmptyConstructorDelegate GetConstructorMethod(Type type)
        {
            EmptyConstructorDelegate emptyConstructorFunction;
            if (_constructorMethods.TryGetValue(type, out emptyConstructorFunction))
            {
                return emptyConstructorFunction;
            }

            emptyConstructorFunction = GetConstructorMethodToCache(type);

            Dictionary<Type, EmptyConstructorDelegate> snapshot;
            Dictionary<Type, EmptyConstructorDelegate> newCache;

            do
            {
                snapshot = _constructorMethods;
                newCache = new Dictionary<Type, EmptyConstructorDelegate>(_constructorMethods);
                newCache[type] = emptyConstructorFunction;
            } 
            while (!ReferenceEquals(Interlocked.CompareExchange(ref _constructorMethods, newCache, snapshot), snapshot));

            return emptyConstructorFunction;
        }
        
        #region Internal Classes

        /// <summary>
        /// Static class that searches for the constructor only once
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private static class ConstructorProvider<T>
        {
            public static readonly EmptyConstructorDelegate EmptyConstructorFunction;

            static ConstructorProvider()
            {
                EmptyConstructorFunction = GetConstructorMethodToCache(typeof(T));
            }
        }

        #endregion
    }
}
