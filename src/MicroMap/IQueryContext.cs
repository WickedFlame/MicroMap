using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MicroMap
{
    public interface IQueryContext<T>
    {
        ComponentContainer Components { get; }

        QueryContext<T> Add(IQueryComponent queryComponent);




        IEnumerable<T> Select();

        IEnumerable<T1> Select<T1>();

        IEnumerable<T1> Select<T1>(Expression<Func<T, T1>> expression);

        IEnumerable<T1> Select<T1>(Func<T, object> expression);

        IEnumerable<T1> Select<T1>(string expression);

        T Single();

        T1 Single<T1>();

        T1 Single<T1>(Func<T, T1> expression);
        
        T1 Single<T1>(Func<T, object> expression);

        T1 Single<T1>(string expression);

        void Update<T1>(T1 expression);

        void Delete();

        void Insert<T1>(Func<T1> p);
    }

    public interface IQueryContext
    {
        ComponentContainer Components { get; }

        QueryContext Add(IQueryComponent queryComponent);



        IEnumerable<T> Select<T>();

        IEnumerable<T1> Select<T1>(Func<T1> definingType);

        IEnumerable<T1> Select<T1>(Func<T1> definingType, Func<T1, object> output);
    }
}
