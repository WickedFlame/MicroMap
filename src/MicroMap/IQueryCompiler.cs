namespace MicroMap
{
    public interface IQueryCompiler
    {
        CompiledQuery Compile<T>(ComponentContainer container);
    }
}
