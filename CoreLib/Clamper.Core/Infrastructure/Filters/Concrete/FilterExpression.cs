namespace Clamper.Core.Infrastructure.Filters.Concrete
{
    public enum FilterExpressionType
    {
        And,
        Or,
        Column,
        Start,
        End,

        EqualsTo,
        NotEqualsTo,
        Contains,
        NotContains,
        StartsWith,
        NotStartsWith,
        EndsWith,
        NotEndsWith,
        IsEmpty,
        IsNotEmpty,
        IsNull,
        IsNotNull,
        GreaterThan,
        LessThan,
        GreaterThanOrEquals,
        LessThanOrEqual,
        Between,
        IsTrue,
        IsFalse,
        In,
        NotIn

    }
    public class FilterExpression
    {
        internal FilterExpression(string column, FilterExpressionType type, object value, bool quote)
        {
            Column = column;
            Type = type;
            Value = value;
            Quote = quote;
        }

        internal FilterExpression(FilterExpressionType type)
        {
            Type = type;
        }

        public string Column { get; }
        public FilterExpressionType Type { get; }
        public object Value { get; }
        public bool Quote { get; }
    }
}
