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
        /// <summary>
        /// Execute the sql string to the RDBMS
        /// </summary>
        /// <param name="query">The query string</param>
        /// <returns>The contex containing the IDataReader</returns>
        public IDataReaderContext Execute(CompiledQuery query)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Execute the sql string to the RDBMS
        /// </summary>
        /// <param name="query">The query string</param>
        /// <returns>The amount of afected rows</returns>
        public int ExecuteNonQuery(CompiledQuery query)
        {
            throw new NotImplementedException();
        }
    }
}
