using System.Linq;

namespace MicroMap
{
    public class QueryCompiler : IQueryCompiler
    {
        public CompiledQuery Compile<T>(ComponentContainer container)
        {
            var items = container.OrderBy(c => (int)c.Type);
            var result = items.Select(i => i.Expression).Aggregate((i, j) => i + " " + j);

            return new CompiledQuery { Query = result };
        }
    }
}
