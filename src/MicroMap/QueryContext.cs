using MicroMap.TMP.Sql;
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

        public QueryContext Add(IQueryComponent queryComponent)
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

        public QueryContext<T> Add(IQueryComponent queryComponent)
        {
            AddComponent(queryComponent);

            return this;
        }






        

        public IEnumerable<T> Select()
        {
            Add(new QueryComponent(SyntaxComponent.Command, "SELECT"));
            Add(new QueryComponent(SyntaxComponent.Keyword, "FROM"));

            // add all fields from type T as FieldList
            var fieldList = typeof(T).GetTypeDefinitionMemberNames();
            var fields = fieldList.Aggregate((i, j) => i + ", " + j);
            Add(new QueryComponent(SyntaxComponent.FieldList, fields));

            return Kernel.Execute<T>(Components);
        }
        
        public IEnumerable<T1> Select<T1>()
        {
            Add(new QueryComponent(SyntaxComponent.Command, "SELECT"));
            Add(new QueryComponent(SyntaxComponent.Keyword, "FROM"));

            // add all fields from type T as FieldList
            var fieldList = typeof(T).GetTypeDefinitionMemberNames();
            var fields = fieldList.Aggregate((i, j) => i + ", " + j);
            Add(new QueryComponent(SyntaxComponent.FieldList, fields));

            return Kernel.Execute<T1>(Components);
        }

        public IEnumerable<T1> Select<T1>(Expression<Func<T, T1>> expression)
        {
            Add(new QueryComponent(SyntaxComponent.Command, "SELECT"));
            Add(new QueryComponent(SyntaxComponent.Keyword, "FROM"));

            var fields = LambdaToSqlCompiler.Compile(expression);
            Add(new QueryComponent(SyntaxComponent.FieldList, fields));

            return Kernel.Execute<T1>(Components);
        }

        public IEnumerable<T1> Select<T1>(Func<T, object> expression)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T1> Select<T1>(string expression)
        {
            Add(new QueryComponent(SyntaxComponent.Command, "SELECT"));
            Add(new QueryComponent(SyntaxComponent.Keyword, "FROM"));
            
            Add(new QueryComponent(SyntaxComponent.FieldList, expression));

            return Kernel.Execute<T1>(Components);
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
    }
}
