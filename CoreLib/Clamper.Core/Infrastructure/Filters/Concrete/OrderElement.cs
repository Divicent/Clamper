using Clamper.Core.Infrastructure.Filters.Abstract;

namespace Clamper.Core.Infrastructure.Filters.Concrete
{
    public class OrderElement<T, TQ> : IOrderElement<T, TQ> where T : IOrderContext
    {
        private readonly string _propertyName;
        private readonly T _parent;
        private readonly TQ _q;

        public OrderElement(string propertyName, T parent, TQ q)
        {
            _parent = parent;
            _propertyName = propertyName;
            _q = q;
        }

        public IOrderJoin<T, TQ> Ascending()
        {
            _parent.Add( new OrderExpression(_propertyName, OrderType.Ascending));
            return new OrderJoin<T, TQ>(_parent, _q);
        }

        public IOrderJoin<T, TQ> Descending()
        {
            _parent.Add(new OrderExpression(_propertyName, OrderType.Descending));
            return new OrderJoin<T, TQ>(_parent, _q);
        }
    }
}

