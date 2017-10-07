using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMap
{

    // THE KERNEL EXECUTES AGAINST THE EXECUTION CONTEXT AND MAPPS THE DATAREADER TO THE OBJECT WITH THE MAPPER


    public interface IExecutionKernel
    {        
        /// <summary>
        /// Executes the query against a RDBMS
        /// </summary>
        /// <typeparam name="T">The expected return type</typeparam>
        /// <param name="query">The query that will be executed</param>
        /// <returns>A list of T</returns>
        IEnumerable<T> Execute<T>(ComponentContainer query);

        /// <summary>
        /// Executes the query against a RDBMS without retrieving a result
        /// </summary>
        /// <param name="query">The query that will be executed</param>
        void ExecuteNonQuery(ComponentContainer query);
    }
}
