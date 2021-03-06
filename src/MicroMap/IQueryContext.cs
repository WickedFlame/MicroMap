﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MicroMap
{
    public interface IQueryContext
    {
        ComponentContainer Components { get; }

        IQueryContext Add(IQueryComponent queryComponent);



        IEnumerable<T> Select<T>();

        IEnumerable<T1> Select<T1>(Func<T1> definingType);

        IEnumerable<T1> Select<T1>(Func<T1> definingType, Func<T1, object> output);
    }

    public interface IQueryContext<T>
    {
        ComponentContainer Components { get; }

        IQueryContext<T> Add(IQueryComponent queryComponent);




        IEnumerable<T> Select();

        IEnumerable<T1> Select<T1>();

        IEnumerable<T1> Select<T1>(Expression<Func<T, T1>> expression);

        IEnumerable<T1> Select<T1>(Expression<Func<T, object>> expression);

        IEnumerable<T1> Select<T1>(string expression);

        T Single();

        T1 Single<T1>();

        T1 Single<T1>(Func<T, T1> expression);
        
        T1 Single<T1>(Func<T, object> expression);

        T1 Single<T1>(string expression);

        void Update<T1>(T1 expression);

        void Delete();

        void Insert<T1>(Func<T1> p);




        /// EXPERIMENTAL
        IQueryContext<T, T1> Join<T1>(Func<T, T1, object> func);
    }

    /// EXPERIMENTAL
    public interface IQueryContext<T, T1>
    {
        /// EXPERIMENTAL
        IEnumerable<T2> Select<T2>(Expression<Func<T, T1, T2>> expression);
    }
}
