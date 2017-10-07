using System;

namespace MicroMap
{
    public interface IDatabaseConnection
    {
        IDatabaseContext Open();
    }

    public class DatabaseConnection : IDatabaseConnection
    {
        private IQueryCompiler _compiler;
        private IExecutionContext _executionContext;

        public DatabaseConnection()
        {
        }

        public DatabaseConnection(IQueryCompiler compiler, IExecutionContext executionContext)
        {
            _compiler = compiler;
            _executionContext = executionContext;
        }
        
        /// <summary>
        /// Opens a new DatabaseContext based on this connection
        /// </summary>
        /// <returns></returns>
        public IDatabaseContext Open()
        {
            var context = new DatabaseContext(this);

            if (_compiler != null)
            {
                context.Compiler = _compiler;
            }

            if (_executionContext != null)
            {
                context.ExecutionContext = _executionContext;
            }

            return context;
        }
    }
}