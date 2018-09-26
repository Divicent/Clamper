using System.Linq;
using Clamper.Core.Infrastructure.Filters.Abstract;

namespace Clamper.Core.Infrastructure.Filters.Concrete
{
    public class NumberFilter<T, TQ> : INumberFilter<T, TQ> where T : IFilterContext
    {
        private readonly string _propertyName;
        private readonly T _parent;
        private readonly TQ _q;

        public NumberFilter(string propertyName, T parent, TQ q)
        {
            _parent = parent;
            _propertyName = propertyName;
            _q = q;
        }

        public IExpressionJoin<T, TQ> EqualsTo(double number)
        {
            _parent.Add(QueryMaker.EqualsTo(_propertyName, number, false));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> NotEquals(double number)
        {
            _parent.Add(QueryMaker.NotEquals(_propertyName, number, false));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> LargerThan(double number)
        {
            _parent.Add(QueryMaker.GreaterThan(_propertyName,number, false));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> LessThan(double number)
        {
            _parent.Add(QueryMaker.LessThan(_propertyName, number, false));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> LargerThanOrEqualTo(double number)
        {
            _parent.Add(QueryMaker.GreaterThanOrEquals(_propertyName, number, false));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> LessThanOrEqualTo(double number)
        {
            _parent.Add(QueryMaker.LessThanOrEquals(_propertyName, number, false));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> Between(double from, double to)
        {
            _parent.Add(QueryMaker.Between(_propertyName, from, to, false));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> IsNull()
        {
            _parent.Add(QueryMaker.IsNull(_propertyName));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> IsNotNull()
        {
            _parent.Add(QueryMaker.IsNotNull(_propertyName));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

		public IExpressionJoin<T, TQ> IsNull(bool isNull)
        {
			_parent.Add(isNull ? QueryMaker.IsNull(_propertyName) : QueryMaker.IsNotNull(_propertyName));
			return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> In(params double[] items)
        {
            _parent.Add(QueryMaker.In(_propertyName, items.Cast<object>().ToArray(), false));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }

        public IExpressionJoin<T, TQ> NotIn(params double[] items)
        {
            _parent.Add(QueryMaker.NotIn(_propertyName, items.Cast<object>().ToArray(), false));
            return new ExpressionJoin<T, TQ>(_parent, _q);
        }
    } 
}

