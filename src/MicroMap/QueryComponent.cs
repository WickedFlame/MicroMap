namespace MicroMap
{
    public class QueryComponent : IQueryComponent
    {
        private readonly SyntaxComponent _type;
        private readonly string _expression;

        public QueryComponent(SyntaxComponent type, string expression)
        {
            _type = type;
            _expression = expression;
        }

        public SyntaxComponent Type => _type;

        public string Expression => _expression;
    }
}