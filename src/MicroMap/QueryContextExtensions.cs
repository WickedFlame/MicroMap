using System;

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

        public static IQueryContext<T> Add<T>(this IQueryContext<T> context, Func<IQueryComponent> expression)
        {
            return context.Add(expression());
        }
    }
}
