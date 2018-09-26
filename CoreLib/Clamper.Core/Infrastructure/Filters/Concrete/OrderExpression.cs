namespace Clamper.Core.Infrastructure.Filters.Concrete
{
    internal enum OrderType
    {
       Ascending,
       Descending
    }

    public class OrderExpression
    {
        internal OrderExpression(string column, OrderType type)
        {
            Column = column;
            Type = type;
        }

        public string Column { get; }
        internal OrderType Type { get; }
    }
}
