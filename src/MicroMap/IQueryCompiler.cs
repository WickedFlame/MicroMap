namespace MicroMap
{
    public interface IQueryCompiler
    {
        CompiledQuery Compile(ComponentContainer container);
    }
}
