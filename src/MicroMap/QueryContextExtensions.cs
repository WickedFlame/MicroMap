using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMap
{
    internal static class QueryContextExtensions
    {
        public static IQueryContext<T> AddSelect<T>(this IQueryContext<T> context)
        {
            return context.Add(new QueryComponent(SyntaxComponent.Command, "SELECT"));
        }

        public static IQueryContext<T> AddFrom<T>(this IQueryContext<T> context)
        {
            return context.Add(new QueryComponent(SyntaxComponent.Keyword, "FROM"));
        }
    }
}
