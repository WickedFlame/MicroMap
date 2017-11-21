using MicroMap.Sql;
using MicroMap.TypeDefinition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MicroMap
{
    public class QueryContextBase
    {
        private readonly ComponentContainer _components;
        private readonly IExecutionKernel _kernel;

        public QueryContextBase(IExecutionKernel kernel)
        {
            _components = new ComponentContainer();
            _kernel = kernel;
        }

        public ComponentContainer Components => _components;

        public IExecutionKernel Kernel => _kernel;

        protected void AddComponent(IQueryComponent queryComponent)
        {
            // ensure the component does not exist yet
            if (_components.Any(c => c.Type == queryComponent.Type))
            {
                throw new InvalidOperationException($"The QueryContext already containes a component with the Type {queryComponent.Type}");
            }

            _components.Add(queryComponent);
        }
    }

    public class QueryContext : QueryContextBase, IQueryContext
    {
        public QueryContext(IExecutionKernel kernel) : base(kernel)
        {
        }

        public IQueryContext Add(IQueryComponent queryComponent)
        {
            AddComponent(queryComponent);

            return this;
        }


        public IEnumerable<T> Select<T>()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T1> Select<T1>(Func<T1> definingType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T1> Select<T1>(Func<T1> definingType, Func<T1, object> output)
        {
            throw new NotImplementedException();
        }
    }

    public class QueryContext<T> : QueryContextBase, IQueryContext<T>
    {
        public QueryContext(IExecutionKernel kernel) : base(kernel)
        {
        }

        public IQueryContext<T> Add(IQueryComponent queryComponent)
        {
            AddComponent(queryComponent);

            return this;
        }



        


        

        public IEnumerable<T> Select()
        {
            var context = this.AddSelect()
                .AddFrom()
                .Add(() =>
                {
                    // add all fields from type T as FieldList
                    var fieldList = typeof(T).GetTypeDefinitionMemberNames();
                    var fields = fieldList.Aggregate((i, j) => i + ", " + j);
                    return new QueryComponent(SyntaxComponent.FieldList, fields);
                });

            return Kernel.Execute<T>(context.Components);
        }
        
        public IEnumerable<T1> Select<T1>()
        {
            var context = this.AddSelect()
                .AddFrom()
                .Add(() =>
                {
                    // add all fields from type T as FieldList
                    var fieldList = typeof(T).GetTypeDefinitionMemberNames();
                    var fields = fieldList.Aggregate((i, j) => i + ", " + j);
                    return new QueryComponent(SyntaxComponent.FieldList, fields);
                });

            return Kernel.Execute<T1>(context.Components);
        }

        public IEnumerable<T1> Select<T1>(Expression<Func<T, T1>> expression)
        {
            var context = this.AddSelect()
                .AddFrom()
                .Add(new QueryComponent(SyntaxComponent.FieldList, LambdaToSqlCompiler.Compile(expression)));

            return Kernel.Execute<T1>(context.Components);
        }

        public IEnumerable<T1> Select<T1>(Expression<Func<T, object>> expression)
        {
            var context = this.AddSelect()
                .AddFrom()
                .Add(new QueryComponent(SyntaxComponent.FieldList, LambdaToSqlCompiler.Compile(expression)));

            return Kernel.Execute<T1>(context.Components);
        }

        public IEnumerable<T1> Select<T1>(string expression)
        {
            var context = this.AddSelect()
                .AddFrom()
                .Add(new QueryComponent(SyntaxComponent.FieldList, expression));

            return Kernel.Execute<T1>(context.Components);
        }

        public T Single()
        {
            throw new NotImplementedException();
        }

        public T1 Single<T1>()
        {
            throw new NotImplementedException();
        }

        public T1 Single<T1>(Func<T, T1> expression)
        {
            throw new NotImplementedException();
        }

        public T1 Single<T1>(Func<T, object> expression)
        {
            throw new NotImplementedException();
        }

        public T1 Single<T1>(string expression)
        {
            throw new NotImplementedException();
        }

        public void Update<T1>(T1 expression)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void Insert<T1>(Func<T1> p)
        {
            throw new NotImplementedException();
        }




        /// EXPERIMENTAL
        public IQueryContext<T, T1> Join<T1>(Func<T, T1, object> func)
        {
            throw new NotImplementedException("EXPERIMENTAL!!");
        }
    }
}
