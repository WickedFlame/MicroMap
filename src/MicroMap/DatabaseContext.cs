using System;
using System.Linq.Expressions;

namespace MicroMap
{
    /// <summary>
    /// [SELECT ] [ID, Name ] [FROM   ] [Table   ] [WHERE ID = 5]
    /// [Command] [FieldList] [Keyword] [Keytable] [Restriction]
    /// </summary>
    public enum SyntaxComponent
    {
        /// <summary>
        /// SELECT, DELETE, UPDATE, INSERT
        /// </summary>
        Command,

        /// <summary>
        /// Comma separated list of desired fields
        /// </summary>
        FieldList,

        /// <summary>
        /// FROM, INTO
        /// </summary>
        Keyword,

        /// <summary>
        /// Table name
        /// </summary>
        Keytable,

        /// <summary>
        /// WHERE, GROUP BY
        /// </summary>
        Restriction
    }

    public class DatabaseContext : IDatabaseContext
    {
        
        private IDatabaseConnection _databaseConnection;
        
        internal DatabaseContext(IDatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;

            Compiler = new QueryCompiler();
            ExecutionContext = new ExecutionContext();
        }

        public IQueryCompiler Compiler { get; set; }

        public IExecutionContext ExecutionContext { get; set; }



        public IQueryContext<T> From<T>(Expression<Func<T, bool>> expression)
        {
            var query = new QueryContext<T>(new ExecutionKernel(Compiler, ExecutionContext));
            query.Add(new QueryComponent(SyntaxComponent.Keytable, $"{typeof(T).Name}"));

            // convert expression to string
            var qstr = MicroMap.TMP.Sql.LambdaToSqlCompiler.Compile(expression);
            query.Add(new QueryComponent(SyntaxComponent.Restriction, $"WHERE {qstr}"));

            return query;
        }

        public IQueryContext From(string expression)
        {
            var query = new QueryContext(new ExecutionKernel(Compiler, ExecutionContext));

            // convert expression to string
            query.Add(new QueryComponent(SyntaxComponent.Restriction, expression));

            return query;
        }

        public IQueryContext<T> From<T>()
        {
            var query = new QueryContext<T>(new ExecutionKernel(Compiler, ExecutionContext));
            query.Add(new QueryComponent(SyntaxComponent.Keytable, $"{typeof(T).Name}"));

            return query;
        }

        #region IDisposeable Implementation

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        internal bool IsDisposed { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases resources held by the object.
        /// </summary>
        public virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                if (disposing && !IsDisposed)
                {
                    // commit all uncommited transactions
                    //Commit();

                    //Kernel.Dispose();

                    IsDisposed = true;
                }
            }
        }

        #endregion
    }
}
