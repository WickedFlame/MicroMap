using MicroMap.Mapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroMap
{

    // THE KERNEL EXECUTES AGAINST THE EXECUTION CONTEXT AND MAPPS THE DATAREADER TO THE OBJECT WITH THE MAPPER


    public class ExecutionKernel : IExecutionKernel
    {
        private readonly IQueryCompiler _compiler;
        private readonly IExecutionContext _executionContext;
        private readonly ObjectMapper _mapper;

        public ExecutionKernel(IQueryCompiler compiler, IExecutionContext executionContext)
        {
            _compiler = compiler;
            _executionContext = executionContext;

            _mapper = new ObjectMapper(new Settings());
        }

        public IEnumerable<T> Execute<T>(ComponentContainer queryContext)
        {
            var query = _compiler.Compile<T>(queryContext);

            // Log the query here
            System.Diagnostics.Debug.WriteLine(query.Query);

            using (var reader = _executionContext.Execute(query))
            {
                return Map<T>(reader.DataReader);
            }
        }
        
        public void ExecuteNonQuery(ComponentContainer query)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<T> Map<T>(IDataReader dataReader)
        {
            return _mapper.Map<T>(dataReader);
        }
    }
}
