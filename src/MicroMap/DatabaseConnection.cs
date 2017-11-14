using System;
using System.Data;
using System.Data.SqlClient;

namespace MicroMap
{
    public interface IDatabaseConnection
    {
        IDatabaseContext Open();

        IDbConnection Conect();
    }

    public class DatabaseConnection : IDatabaseConnection
    {
        private IQueryCompiler _compiler;
        private IExecutionContext _executionContext;

        private string _connectionString;

        public DatabaseConnection()
        {
        }

        public DatabaseConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DatabaseConnection(IQueryCompiler compiler, IExecutionContext executionContext)
        {
            _compiler = compiler;
            _executionContext = executionContext;
        }

        public IDbConnection Conect()
        {
            return new SqlConnection(_connectionString);
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