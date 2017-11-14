using MicroMap.Reader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMap
{

    // THE EXECUTIONCONTEXT EXECUTES AGAINST THE RDBMS AND RETURNES A STREAM


    public class ExecutionContext : IExecutionContext
    {
        private readonly IDatabaseConnection _dbConnection;

        public ExecutionContext(IDatabaseConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        /// <summary>
        /// Execute the sql string to the RDBMS
        /// </summary>
        /// <param name="query">The query string</param>
        /// <returns>The contex containing the IDataReader</returns>
        public IDataReaderContext Execute(CompiledQuery query)
        {
            var connection = _dbConnection.Conect();
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = query.Query;
            command.Connection = connection;

            return new DataReaderContext(command.ExecuteReader(), connection, command);
        }

        /// <summary>
        /// Execute the sql string to the RDBMS
        /// </summary>
        /// <param name="query">The query string</param>
        /// <returns>The amount of afected rows</returns>
        public int ExecuteNonQuery(CompiledQuery query)
        {
            using (var connection = _dbConnection.Conect())
            {
                connection.Open();
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = query.Query;
                    cmd.Connection = connection;
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
