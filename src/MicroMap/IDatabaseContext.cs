using System;
using System.Linq.Expressions;

namespace MicroMap
{
    public interface IDatabaseContext : IDisposable
    {
        IQueryCompiler Compiler { get; set; }

        IExecutionContext ExecutionContext { get; set; }

        IQueryContext<T> From<T>(Expression<Func<T, bool>> expression);

        IQueryContext<T> From<T>();

        IQueryContext From(string expression);
    }
}