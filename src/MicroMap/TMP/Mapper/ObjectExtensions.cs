using System;

namespace MicroMap.Mapper
{
    internal static class ObjectExtensions
    {
        public static bool IsDBNull(this object obj)
        {
            return obj is DBNull;
        }
    }
}
