using Clamper.Core.Infrastructure.Filters.Abstract;

namespace Clamper.Core.Infrastructure.Filters.Concrete
{
    public class OrderJoin<T, TQ> : IOrderJoin<T, TQ> where T : IOrderContext
    {
        private readonly TQ _q;

        public OrderJoin(T t, TQ q)
        {
            And = t;
            _q = q;
        }

        public T And { get; }

        public TQ Order()
        {
            return _q;
        }
    }
}

