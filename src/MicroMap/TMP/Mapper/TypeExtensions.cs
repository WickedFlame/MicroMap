using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using MicroMap.TypeDefinition;

namespace MicroMap.Mapper
{
    internal static class TypeExtensions
    {
        public static ConstructorInfo GetEmptyConstructor(this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes);
        }

        public static bool HasGenericType(this Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType)
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        [DebuggerStepThrough]
        public static bool IsAnonymousType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        public static Type GetTypeWithGenericTypeDefinitionOfAny(this Type type, params Type[] genericTypeDefinitions)
        {
            foreach (var genericTypeDefinition in genericTypeDefinitions)
            {
                var genericType = type.GetTypeWithGenericTypeDefinitionOf(genericTypeDefinition);
                if (genericType == null && type == genericTypeDefinition)
                {
                    genericType = type;
                }

                if (genericType != null)
                {
                    return genericType;
                }
            }

            return null;
        }
    }
}
