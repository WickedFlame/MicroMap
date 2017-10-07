namespace MicroMap
{
    public interface IQueryComponent
    {
        SyntaxComponent Type { get; }

        string Expression { get; }
    }
}